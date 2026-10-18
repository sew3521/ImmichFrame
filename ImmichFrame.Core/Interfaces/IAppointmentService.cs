using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Interfaces;

public interface ICalendarService
{
    Task<List<IAppointment>> GetAppointments();

    Task<CalendarFetchResult> GetCalendarResult();
}
