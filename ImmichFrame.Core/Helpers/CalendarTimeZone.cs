namespace ImmichFrame.Core.Helpers;

/// <summary>
/// Resolves the timezone used for calendar display.
/// An empty setting follows the container's configured timezone.
/// </summary>
public static class CalendarTimeZone
{
    public static TimeZoneInfo Resolve(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return TimeZoneInfo.Local;
        }

        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    /// <summary>
    /// Returns the current calendar date in the selected timezone.
    /// </summary>
    public static DateTime Today(
        TimeZoneInfo timeZone,
        DateTimeOffset? now = null)
    {
        var instant = now ?? DateTimeOffset.UtcNow;

        return TimeZoneInfo.ConvertTime(
            instant,
            timeZone
        ).Date;
    }

    /// <summary>
    /// Converts an instant into a local wall-clock time in the
    /// selected timezone without attaching the server timezone.
    /// </summary>
    public static DateTime ToLocal(
        DateTimeOffset instant,
        TimeZoneInfo timeZone)
    {
        return DateTime.SpecifyKind(
            TimeZoneInfo.ConvertTime(
                instant,
                timeZone
            ).DateTime,
            DateTimeKind.Unspecified
        );
    }
}
