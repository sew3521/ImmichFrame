using ImmichFrame.Core.Interfaces;

namespace ImmichFrame.Core.Helpers;

public static class CalendarEventDeduplicator
{
    public static List<IAppointment> Deduplicate(
        IEnumerable<IAppointment> appointments)
    {
        var seen = new HashSet<(string Title, DateTime Start, DateTime End)>();
        var result = new List<IAppointment>();

        foreach (var appointment in appointments)
        {
            var key = (
                (appointment.Summary ?? "").Trim().ToUpperInvariant(),
                appointment.StartTime,
                appointment.EndTime
            );

            if (seen.Add(key))
            {
                result.Add(appointment);
            }
        }

        return result;
    }
}
