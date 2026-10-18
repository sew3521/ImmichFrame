using ImmichFrame.Core.Interfaces;
using ImmichFrame.Core.Models;

namespace ImmichFrame.Core.Helpers;

public sealed class CalendarFallbackCache
{
    private readonly Dictionary<Guid, Entry> _entries = new();
    private readonly TimeSpan _maxAge;

    private sealed record Entry(
        string Url,
        string Name,
        string Color,
        DateTime Date,
        int DaysAhead,
        DateTimeOffset LastSuccess,
        List<IAppointment> Appointments);

    public CalendarFallbackCache(TimeSpan? maxAge = null)
    {
        _maxAge = maxAge ?? TimeSpan.FromHours(2);
    }

    public void Store(
        CalendarSource source,
        DateTime date,
        int daysAhead,
        IEnumerable<IAppointment> appointments,
        DateTimeOffset now)
    {
        _entries[source.Id] = new Entry(
            source.Url,
            source.Name,
            source.Color,
            date.Date,
            daysAhead,
            now,
            appointments.ToList());
    }

    public List<IAppointment>? GetFallback(
        CalendarSource source,
        DateTime date,
        int daysAhead,
        DateTimeOffset now)
    {
        if (!_entries.TryGetValue(source.Id, out var entry))
        {
            return null;
        }

        if (entry.Url != source.Url ||
            entry.Name != source.Name ||
            entry.Color != source.Color ||
            entry.Date != date.Date ||
            entry.DaysAhead != daysAhead ||
            now - entry.LastSuccess > _maxAge)
        {
            _entries.Remove(source.Id);
            return null;
        }

        return entry.Appointments.ToList();
    }

    public void RetainOnly(IEnumerable<CalendarSource> sources)
    {
        var enabled = sources
            .Where(x => x.Enabled)
            .Select(x => x.Id)
            .ToHashSet();

        foreach (var id in _entries.Keys.ToList())
        {
            if (!enabled.Contains(id))
            {
                _entries.Remove(id);
            }
        }
    }
}
