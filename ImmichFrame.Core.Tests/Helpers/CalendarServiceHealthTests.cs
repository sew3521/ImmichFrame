using System.Net;
using ImmichFrame.Core.Models;
using ImmichFrame.Core.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarServiceHealthTests
{
    private sealed class FakeHandler(
        Func<HttpRequestMessage, HttpResponseMessage> respond)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(respond(request));
        }
    }

    private static IcalCalendarService CreateService(
        IEnumerable<CalendarSource> calendars,
        Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var client = new HttpClient(new FakeHandler(respond));

        var factory = new Mock<IHttpClientFactory>();

        factory.Setup(x => x.CreateClient("ImmichFrameCalendar"))
            .Returns(client);

        var settings = new Mock<IGeneralSettings>();

        settings.SetupGet(x => x.Calendars)
            .Returns(calendars.ToList());

        settings.SetupGet(x => x.Webcalendars)
            .Returns(new List<string>());

        settings.SetupGet(x => x.CalendarDaysAhead)
            .Returns(0);

        return new IcalCalendarService(
            settings.Object,
            NullLogger<IcalCalendarService>.Instance,
            factory.Object,
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(100));
    }

    private static CalendarSource Source(string name) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Url = $"https://example.com/{name}.ics"
    };

    private static HttpResponseMessage Success(string title)
    {
        var today = DateTime.Today;

        var ical =
            "BEGIN:VCALENDAR\r\n" +
            "VERSION:2.0\r\n" +
            "BEGIN:VEVENT\r\n" +
            $"UID:{Guid.NewGuid()}@test\r\n" +
            $"DTSTAMP:{today:yyyyMMdd}T120000Z\r\n" +
            $"DTSTART;VALUE=DATE:{today:yyyyMMdd}\r\n" +
            $"DTEND;VALUE=DATE:{today.AddDays(1):yyyyMMdd}\r\n" +
            $"SUMMARY:{title}\r\n" +
            "END:VEVENT\r\n" +
            "END:VCALENDAR\r\n";

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ical)
        };
    }

    [Test]
    public async Task SuccessfulCalendar_ReportsAvailable()
    {
        var calendars = new List<CalendarSource> { Source("Stephen") };

        var service = CreateService(calendars, _ => Success("Dinner"));

        var result = await service.GetCalendarResult();

        Assert.Multiple(() =>
        {
            Assert.That(result.Appointments, Has.Count.EqualTo(1));
            Assert.That(result.CalendarStatuses, Has.Count.EqualTo(1));
            Assert.That(result.CalendarStatuses[0].Available, Is.True);
            Assert.That(result.CalendarStatuses[0].ShowWarning, Is.False);
        });
    }

    [Test]
    public async Task FailedCalendar_ReportsWarning()
    {
        var calendars = new List<CalendarSource> { Source("Emily") };

        var service = CreateService(
            calendars,
            _ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        var result = await service.GetCalendarResult();

        Assert.Multiple(() =>
        {
            Assert.That(result.Appointments, Is.Empty);
            Assert.That(result.CalendarStatuses, Has.Count.EqualTo(1));
            Assert.That(result.CalendarStatuses[0].Available, Is.False);
            Assert.That(result.CalendarStatuses[0].ShowWarning, Is.True);
        });
    }

    [Test]
    public async Task MixedCalendars_OnlyFailedCalendarShowsWarning()
    {
        var stephen = Source("Stephen");
        var emily = Source("Emily");

        var calendars = new List<CalendarSource> { stephen, emily };

        var service = CreateService(calendars, request =>
            request.RequestUri!.AbsolutePath.Contains("Stephen")
                ? Success("Meeting")
                : new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        var result = await service.GetCalendarResult();

        Assert.Multiple(() =>
        {
            Assert.That(result.Appointments, Has.Count.EqualTo(1));
            Assert.That(result.CalendarStatuses, Has.Count.EqualTo(2));
            Assert.That(result.CalendarStatuses[0].ShowWarning, Is.False);
            Assert.That(result.CalendarStatuses[1].ShowWarning, Is.True);
        });
    }

    [Test]
    public async Task TemporaryFailure_UsesCachedAppointments()
    {
        var source = Source("Emily");
        var calls = 0;

        var service = CreateService(
            new[] { source },
            _ => ++calls == 1
                ? Success("Dinner")
                : new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

        var first = await service.GetCalendarResult();

        await Task.Delay(150);

        var second = await service.GetCalendarResult();

        Assert.Multiple(() =>
        {
            Assert.That(first.CalendarStatuses[0].Available, Is.True);
            Assert.That(second.Appointments, Has.Count.EqualTo(1));
            Assert.That(second.CalendarStatuses[0].Available, Is.False);
            Assert.That(second.CalendarStatuses[0].UsingFallback, Is.True);
            Assert.That(second.CalendarStatuses[0].ShowWarning, Is.False);
        });
    }

    [Test]
    public async Task RecoveredCalendar_ClearsWarning()
    {
        var source = Source("Emily");
        var calls = 0;

        var service = CreateService(
            new[] { source },
            _ => ++calls == 1
                ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                : Success("Recovered Event"));

        var first = await service.GetCalendarResult();

        await Task.Delay(150);

        var second = await service.GetCalendarResult();

        Assert.Multiple(() =>
        {
            Assert.That(first.CalendarStatuses[0].ShowWarning, Is.True);
            Assert.That(second.CalendarStatuses[0].Available, Is.True);
            Assert.That(second.CalendarStatuses[0].ShowWarning, Is.False);
            Assert.That(second.Appointments, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task EmptyCalendar_DoesNotShowWarning()
    {
        var source = Source("Emily");

        var emptyIcal =
            "BEGIN:VCALENDAR\r\n" +
            "VERSION:2.0\r\n" +
            "PRODID:-//ImmichFrame Tests//EN\r\n" +
            "END:VCALENDAR\r\n";

        var service = CreateService(
            new[] { source },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(emptyIcal)
            });

        var result = await service.GetCalendarResult();

        Assert.Multiple(() =>
        {
            Assert.That(result.Appointments, Is.Empty);
            Assert.That(result.CalendarStatuses, Has.Count.EqualTo(1));
            Assert.That(result.CalendarStatuses[0].Available, Is.True);
            Assert.That(result.CalendarStatuses[0].ShowWarning, Is.False);
        });
    }

}
