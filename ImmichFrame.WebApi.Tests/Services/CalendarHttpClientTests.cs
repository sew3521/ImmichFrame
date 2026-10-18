using Microsoft.AspNetCore.TestHost;
using ImmichFrame.WebApi.Tests.Mocks;
using ImmichFrame.WebApi.Models;
using System.Net.Http.Headers;
using System.Net;
using ImmichFrame.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Services;

[TestFixture]
public class CalendarHttpClientTests
{
    [Test]
    public void CalendarService_ResolvesFromDependencyInjection()
    {
        using var factory = new WebApplicationFactory<Program>();

        var service = factory.Services
            .GetRequiredService<ICalendarService>();

        Assert.That(service, Is.Not.Null);
        Assert.That(service, Is.InstanceOf<IcalCalendarService>());
    }

    [Test]
    public async Task CalendarStatus_WithoutConfiguredSecret_ReturnsJsonArray()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Calendar/Status");

        Assert.That(
            response.StatusCode,
            Is.EqualTo(System.Net.HttpStatusCode.OK));

        var json = await response.Content.ReadAsStringAsync();

        Assert.That(json.TrimStart(), Does.StartWith("["));
        Assert.That(json.TrimEnd(), Does.EndWith("]"));
    }

    [TestCase(null, HttpStatusCode.Unauthorized)]
    [TestCase("incorrect-token", HttpStatusCode.Unauthorized)]
    [TestCase("calendar-test-secret", HttpStatusCode.OK)]
    public async Task CalendarStatus_WithConfiguredSecret(
        string? token,
        HttpStatusCode expectedStatus)
    {
        var generalSettings = new GeneralSettings
        {
            AuthenticationSecret = "calendar-test-secret",
            Calendars = new()
        };

        var serverSettings = new ServerSettings
        {
            GeneralSettingsImpl = generalSettings,
            AccountsImpl = new List<ServerAccountSettings>()
        };

        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IServerSettings>(serverSettings);
                    services.AddSingleton<IGeneralSettings>(generalSettings);
                });
            });

        using var client = factory.CreateClient();

        if (token != null)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.GetAsync("/api/Calendar/Status");

        Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));

        if (expectedStatus == HttpStatusCode.OK)
        {
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(json.TrimStart(), Does.StartWith("["));
            Assert.That(json.TrimEnd(), Does.EndWith("]"));
        }
    }

    [Test]
    public void CalendarClient_IsRegistered()
    {
        using var factory = new WebApplicationFactory<Program>();

        var clientFactory = factory.Services
            .GetRequiredService<IHttpClientFactory>();

        using var client = clientFactory.CreateClient(
            "ImmichFrameCalendar");

        Assert.That(client, Is.Not.Null);
    }
    [Test]
    public async Task CalendarClient_DoesNotFollowRedirect()
    {
        using var target = new System.Net.Sockets.TcpListener(
            System.Net.IPAddress.Loopback, 0);
        target.Start();

        var targetPort = ((System.Net.IPEndPoint)
            target.LocalEndpoint).Port;

        using var origin = new System.Net.Sockets.TcpListener(
            System.Net.IPAddress.Loopback, 0);
        origin.Start();

        var originPort = ((System.Net.IPEndPoint)
            origin.LocalEndpoint).Port;

        var originTask = Task.Run(async () =>
        {
            using var connection = await origin.AcceptTcpClientAsync();
            await using var stream = connection.GetStream();

            var buffer = new byte[4096];
            await stream.ReadAsync(buffer);

            var response =
                "HTTP/1.1 302 Found\r\n" +
                $"Location: http://127.0.0.1:{targetPort}/redirected\r\n" +
                "Content-Length: 0\r\n" +
                "Connection: close\r\n\r\n";

            var bytes = System.Text.Encoding.ASCII.GetBytes(response);
            await stream.WriteAsync(bytes);
        });

        using var factory = new WebApplicationFactory<Program>();

        var clientFactory = factory.Services
            .GetRequiredService<IHttpClientFactory>();

        using var client = clientFactory.CreateClient(
            "ImmichFrameCalendar");

        using var response = await client.GetAsync(
            $"http://127.0.0.1:{originPort}/calendar.ics");

        await originTask;

        Assert.That(
            response.StatusCode,
            Is.EqualTo(System.Net.HttpStatusCode.Found));

        Assert.That(target.Pending(), Is.False,
            "Redirect destination should not receive a request.");
    }

}
