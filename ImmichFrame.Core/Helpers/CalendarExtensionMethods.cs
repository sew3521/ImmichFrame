using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using ImmichFrame.Core.Interfaces;
using ImmichFrame.Core.Models;

namespace ImmichFrame.WebApi.Helpers
{
    public static class CalendarExtensionMethods
    {
        public static IAppointment ToAppointment(
            this Occurrence occurrence,
            TimeZoneInfo? timeZone = null)
        {
            var calEvent = occurrence.Source as CalendarEvent;
            var zone = timeZone ?? TimeZoneInfo.Local;

            return new Appointment
            {
                Summary = calEvent?.Summary ?? "",
                Description = calEvent?.Description ?? "",
                StartTime = ImmichFrame.Core.Helpers.CalendarAppointmentTime.Convert(
                    occurrence.Period.StartTime, zone),
                Duration = occurrence.Period.Duration,
                EndTime = ImmichFrame.Core.Helpers.CalendarAppointmentTime.Convert(
                    occurrence.Period.EndTime, zone),
                Location = calEvent?.Location ?? ""
            };
        }

        public static IAppointment ToAppointment(
            this CalendarEvent calEvent,
            TimeZoneInfo? timeZone = null)
        {
            var zone = timeZone ?? TimeZoneInfo.Local;

            return new Appointment
            {
                Summary = calEvent.Summary,
                Description = calEvent.Description,
                StartTime = ImmichFrame.Core.Helpers.CalendarAppointmentTime.Convert(
                    calEvent.Start, zone),
                Duration = calEvent.Duration,
                EndTime = ImmichFrame.Core.Helpers.CalendarAppointmentTime.Convert(
                    calEvent.End, zone),
                Location = calEvent.Location
            };
        }
    }
}
