using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ImmichFrame.Core.Models;

namespace ImmichFrame.Core.Helpers;

public static class CalendarCacheKey
{
    public static string Create(
        DateTime today,
        int daysAhead,
        IEnumerable<CalendarSource> calendars,
        string? timeZoneId = null)
    {
        var configuration = calendars.Select(x => new
        {
            x.Id,
            x.Name,
            x.Url,
            x.Color,
            x.Enabled
        }).ToArray();

        var json = JsonSerializer.Serialize(configuration);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));

        return $"appointments-{today:yyyyMMdd}-{daysAhead}-{timeZoneId ?? "system"}-{Convert.ToHexString(hash)}";
    }
}
