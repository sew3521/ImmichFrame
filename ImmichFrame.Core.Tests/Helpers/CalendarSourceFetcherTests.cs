using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Models;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarSourceFetcherTests
{
    [Test]
    public async Task DisabledCalendar_DoesNotFetch()
    {
        var source = new CalendarSource
        {
            Url = "https://example.com/calendar.ics",
            Enabled = false
        };

        using var client = new HttpClient();

        var result = await CalendarSourceFetcher.FetchAsync(
            source,
            client,
            NullLogger.Instance);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UnsupportedScheme_IsRejected()
    {
        var source = new CalendarSource
        {
            Url = "file:///etc/passwd",
            Enabled = true
        };

        using var client = new HttpClient();

        var result = await CalendarSourceFetcher.FetchAsync(
            source,
            client,
            NullLogger.Instance);

        Assert.That(result, Is.Null);
    }
    [Test]
    public async Task SuccessfulDownload_ReturnsCalendarContent()
    {
        const string calendarData = "BEGIN:VCALENDAR";

        using var client = new HttpClient(
            new FakeHandler(_ =>
                new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(calendarData)
                }));

        var source = new CalendarSource
        {
            Url = "https://example.com/calendar.ics"
        };

        var result = await CalendarSourceFetcher.FetchAsync(
            source, client, NullLogger.Instance);

        Assert.That(result, Is.EqualTo(calendarData));
    }

    [Test]
    public async Task FailedDownload_ReturnsNull()
    {
        using var client = new HttpClient(
            new FakeHandler(_ =>
                new HttpResponseMessage(
                    System.Net.HttpStatusCode.ServiceUnavailable)));

        var source = new CalendarSource
        {
            Url = "https://example.com/calendar.ics"
        };

        var result = await CalendarSourceFetcher.FetchAsync(
            source, client, NullLogger.Instance);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task BasicAuthentication_IsSentWithoutCredentialsInUrl()
    {
        string? authorization = null;
        string? requestedUrl = null;

        using var client = new HttpClient(
            new FakeHandler(request =>
            {
                authorization = request.Headers.Authorization?.ToString();
                requestedUrl = request.RequestUri?.ToString();

                return new HttpResponseMessage(
                    System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("BEGIN:VCALENDAR")
                };
            }));

        var source = new CalendarSource
        {
            Url = "https://username:password@example.com/calendar.ics"
        };

        await CalendarSourceFetcher.FetchAsync(
            source, client, NullLogger.Instance);

        Assert.That(
            authorization,
            Is.EqualTo("Basic " +
                Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(
                        "username:password"))));

        Assert.That(requestedUrl, Does.Not.Contain("username"));
        Assert.That(requestedUrl, Does.Not.Contain("password"));
    }

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

}
