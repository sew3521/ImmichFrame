using Ical.Net;
using ImmichFrame.Core.Interfaces;
using ImmichFrame.WebApi.Helpers;

namespace ImmichFrame.Core.Helpers;

public static class CalendarWindow
{
    public static List<IAppointment> GetAppointments(
        Calendar calendar,
        DateTime today,
        int daysAhead)
    {
        if (daysAhead < 0 || daysAhead > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(daysAhead));
        }

        var start = today.Date;
        var end = start.AddDays(daysAhead + 1);

        return calendar
            .GetOccurrences(start, end)
            .Select(x => x.ToAppointment())
            .Where(x => x.EndTime > start && x.StartTime < end)
            .OrderBy(x => x.StartTime)
            .ToList();
    }
}
