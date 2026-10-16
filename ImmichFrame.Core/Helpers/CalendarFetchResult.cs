using ImmichFrame.Core.Interfaces;

namespace ImmichFrame.Core.Helpers;

public class CalendarFetchResult
{
    public List<IAppointment> Appointments { get; init; } = new();

    public bool Complete { get; init; }
}
