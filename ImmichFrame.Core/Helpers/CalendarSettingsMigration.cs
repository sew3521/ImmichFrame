using ImmichFrame.Core.Models;

namespace ImmichFrame.Core.Helpers;

public static class CalendarSettingsMigration
{
    private static readonly string[] DefaultColors =
    {
        "#6299EC",
        "#E69B69",
        "#A88BDC",
        "#69BFA5",
        "#E5B85C",
        "#E67C96"
    };

    public static List<CalendarSource> Resolve(
        List<CalendarSource>? calendars,
        List<string>? legacyUrls)
    {
        // A non-null list is authoritative, even when empty.
        // This prevents intentionally deleted calendars from returning.
        if (calendars != null)
        {
            return calendars;
        }

        var urls = (legacyUrls ?? new List<string>())
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        return urls.Select((url, index) => new CalendarSource
        {
            Id = Guid.NewGuid(),
            Name = $"Calendar {index + 1}",
            Url = url,
            Color = DefaultColors[index % DefaultColors.Length],
            Enabled = true
        }).ToList();
    }
}
