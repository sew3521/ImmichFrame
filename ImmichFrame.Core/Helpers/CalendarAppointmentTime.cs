using Ical.Net.DataTypes;

namespace ImmichFrame.Core.Helpers;

/// <summary>
/// Converts iCalendar dates into the configured display timezone.
/// All-day and floating times retain their calendar/wall-clock values.
/// </summary>
public static class CalendarAppointmentTime
{
    public static DateTime Convert(
        IDateTime value,
        TimeZoneInfo timeZone)
    {
        // DATE values represent calendar dates, not UTC instants.
        if (!value.HasTime)
        {
            return DateTime.SpecifyKind(
                value.Value.Date,
                DateTimeKind.Unspecified
            );
        }

        // Floating times have no timezone. Treat them as wall-clock
        // times in the configured display timezone.
        if (!value.IsUtc && string.IsNullOrWhiteSpace(value.TzId))
        {
            return DateTime.SpecifyKind(
                value.Value,
                DateTimeKind.Unspecified
            );
        }

        // UTC and explicitly timezone-tagged events represent instants.
        // Convert those instants to the configured display timezone.
        return CalendarTimeZone.ToLocal(
            new DateTimeOffset(value.AsUtc, TimeSpan.Zero),
            timeZone
        );
    }
}
