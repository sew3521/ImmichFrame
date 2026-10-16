using System.Net.Http.Headers;
using System.Text;
using Ical.Net;
using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Interfaces;
using ImmichFrame.WebApi.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class IcalCalendarService : ICalendarService
{
    private readonly IGeneralSettings _serverSettings;
    private readonly ILogger<IcalCalendarService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _appointmentCache =
        new MemoryCache(new MemoryCacheOptions());

    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public IcalCalendarService(IGeneralSettings serverSettings, ILogger<IcalCalendarService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _serverSettings = serverSettings;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<IAppointment>> GetAppointments()
    {
        var today = DateTime.Today;
        var daysAhead = _serverSettings.CalendarDaysAhead;

        var sources = CalendarSettingsMigration.Resolve(
            _serverSettings.Calendars,
            _serverSettings.Webcalendars
        )
        .Where(x => x.Enabled)
        .Select(x => new ImmichFrame.Core.Models.CalendarSource
        {
            Id = x.Id,
            Name = x.Name,
            Url = x.Url,
            Color = x.Color,
            Enabled = x.Enabled
        })
        .ToList();

        var cacheKey = CalendarCacheKey.Create(today, daysAhead, sources);

        if (_appointmentCache.TryGetValue(cacheKey, out CalendarFetchResult? cached)
            && cached != null)
        {
            return cached.Appointments;
        }

        await _cacheLock.WaitAsync();
        try
        {
            if (_appointmentCache.TryGetValue(cacheKey, out cached)
                && cached != null)
            {
                return cached.Appointments;
            }

            var appointments = new List<IAppointment>();
            var complete = true;
            var client = _httpClientFactory.CreateClient("ImmichFrameCalendar");

            foreach (var source in sources)
            {
                var ical = await CalendarSourceFetcher.FetchAsync(
                    source, client, _logger);

                if (ical == null)
                {
                    complete = false;
                    continue;
                }

                try
                {
                    var calendar = Calendar.Load(ical);

                    appointments.AddRange(
                        CalendarWindow.GetAppointments(
                            calendar, today, daysAhead, source)
                    );
                }
                catch (Exception ex)
                {
                    complete = false;

                    _logger.LogWarning(
                        "Calendar {CalendarId} could not be parsed: {ErrorType}.",
                        source.Id,
                        ex.GetType().Name);
                }
            }

            var result = new CalendarFetchResult
            {
                Appointments = appointments
                    .OrderBy(x => x.StartTime)
                    .ToList(),
                Complete = complete
            };

            _appointmentCache.Set(
                cacheKey,
                result,
                complete
                    ? TimeSpan.FromMinutes(15)
                    : TimeSpan.FromMinutes(1));

            return result.Appointments;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<List<string>> GetCalendars(IEnumerable<(string? auth, string url)> calendars)
    {
        var icals = new List<string>();
        var client = _httpClientFactory.CreateClient("ImmichFrameCalendar");

        foreach (var calendar in calendars)
        {
            _logger.LogDebug("Loading legacy calendar.");

            string httpUrl = calendar.url.Replace("webcal://", "https://");

            using var request = new HttpRequestMessage(HttpMethod.Get, httpUrl);

            if (!string.IsNullOrEmpty(calendar.auth))
            {
                var byteArray = Encoding.UTF8.GetBytes(calendar.auth);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                icals.Add(await response.Content.ReadAsStringAsync());
            }
            else
            {
                _logger.LogWarning("Legacy calendar request failed with HTTP {StatusCode}.", (int)response.StatusCode);
            }
        }

        return icals;
    }
}