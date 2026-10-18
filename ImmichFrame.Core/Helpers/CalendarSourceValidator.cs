using System.Text.RegularExpressions;
using ImmichFrame.Core.Models;

namespace ImmichFrame.Core.Helpers;

public static class CalendarSourceValidator
{
    private static readonly Regex ColorPattern = new(
        "^#[0-9A-Fa-f]{6}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static void Validate(IEnumerable<CalendarSource>? calendars)
    {
        if (calendars == null)
        {
            return;
        }

        var seenIds = new HashSet<Guid>();

        foreach (var calendar in calendars)
        {
            if (calendar == null)
            {
                throw new ArgumentException(
                    "Calendar entries cannot be null.");
            }

            if (calendar.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Calendar ID cannot be empty.");
            }

            if (!seenIds.Add(calendar.Id))
            {
                throw new ArgumentException(
                    "Duplicate calendar IDs are not allowed.");
            }

            if (string.IsNullOrWhiteSpace(calendar.Name))
            {
                throw new ArgumentException(
                    "Calendar name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(calendar.Url))
            {
                throw new ArgumentException(
                    $"Calendar '{calendar.Name}' must have a URL.");
            }

            var normalizedUrl = calendar.Url.Replace(
                "webcal://",
                "https://",
                StringComparison.OrdinalIgnoreCase);

            if (!Uri.TryCreate(
                    normalizedUrl,
                    UriKind.Absolute,
                    out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttps &&
                 uri.Scheme != Uri.UriSchemeHttp))
            {
                throw new ArgumentException(
                    $"Calendar '{calendar.Name}' has an invalid URL.");
            }

            if (string.IsNullOrWhiteSpace(calendar.Color) ||
                !ColorPattern.IsMatch(calendar.Color))
            {
                throw new ArgumentException(
                    $"Calendar '{calendar.Name}' has an invalid color. Use #RRGGBB.");
            }
        }
    }
}
