using System.Net.Http.Headers;
using System.Text;
using ImmichFrame.Core.Models;
using Microsoft.Extensions.Logging;

namespace ImmichFrame.Core.Helpers;

public static class CalendarSourceFetcher
{
    public static async Task<string?> FetchAsync(
        CalendarSource source,
        HttpClient client,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (!source.Enabled)
        {
            return null;
        }

        try
        {
            var url = source.Url.Replace(
                "webcal://", "https://",
                StringComparison.OrdinalIgnoreCase);

            var uri = new Uri(url);

            if (uri.Scheme != Uri.UriSchemeHttps &&
                uri.Scheme != Uri.UriSchemeHttp)
            {
                throw new InvalidOperationException(
                    "Unsupported calendar URL scheme.");
            }

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                uri.GetComponents(
                    UriComponents.AbsoluteUri & ~UriComponents.UserInfo,
                    UriFormat.UriEscaped));

            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                var credentials = Uri.UnescapeDataString(uri.UserInfo);

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(credentials)));
            }

            using var response = await client.SendAsync(
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Calendar {CalendarId} returned HTTP {StatusCode}.",
                    source.Id,
                    (int)response.StatusCode);

                return null;
            }

            return await response.Content.ReadAsStringAsync(
                cancellationToken);
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            UriFormatException or
            InvalidOperationException or
            TaskCanceledException)
        {
            logger.LogWarning(
                "Unable to retrieve calendar {CalendarId}: {ErrorType}.",
                source.Id,
                ex.GetType().Name);

            return null;
        }
    }
}
