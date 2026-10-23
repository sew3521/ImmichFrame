using Ical.Net;
using ImmichFrame.Core.Interfaces;
using ImmichFrame.WebApi.Helpers;

namespace ImmichFrame.Core.Helpers;

public static class CalendarWindow
{
    public static List<IAppointment> GetAppointments(
        Calendar calendar,
        DateTime today,
        int daysAhead,
        ImmichFrame.Core.Models.CalendarSource source,
        TimeZoneInfo? timeZone = null)
    {
        var appointments = GetAppointments(
            calendar, today, daysAhead, timeZone);

        foreach (var appointment in appointments)
        {
            appointment.CalendarId = source.Id;
            appointment.CalendarName = source.Name;
            appointment.CalendarColor = source.Color;
        }

        return appointments;
    }

    public static List<IAppointment> GetAppointments(
        Calendar calendar,
        DateTime today,
        int daysAhead,
        TimeZoneInfo? timeZone = null)
    {
        if (daysAhead < 0 || daysAhead > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(daysAhead));
        }

        var zone = timeZone ?? TimeZoneInfo.Local;
        var start = today.Date;
        var end = start.AddDays(daysAhead + 1);

        // Ical.Net evaluates occurrences using its own timezone rules.
        // Expand the search to avoid losing events near timezone boundaries.
        // Final filtering always uses the configured display timezone.
        var searchStart = start.AddDays(-2);
        var searchEnd = end.AddDays(2);

        return calendar
            .GetOccurrences(searchStart, searchEnd)
            .Select(x => x.ToAppointment(zone))
            .Where(x => x.EndTime > start && x.StartTime < end)
            .OrderBy(x => x.StartTime)
            .ToList();
    }
}
