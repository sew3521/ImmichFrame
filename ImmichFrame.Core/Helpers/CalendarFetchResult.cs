using ImmichFrame.Core.Interfaces;
using ImmichFrame.Core.Models;

namespace ImmichFrame.Core.Helpers;

public class CalendarFetchResult
{
    public List<IAppointment> Appointments { get; init; } = new();

    public bool Complete { get; init; }

    public List<CalendarHealthStatus> CalendarStatuses { get; init; } = new();
}
