using Ical.Net;
using ImmichFrame.Core.Helpers;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarAppointmentTimeTests
{
    private static Ical.Net.DataTypes.IDateTime Start(string dtstart)
    {
        var calendar = Calendar.Load(
            "BEGIN:VCALENDAR\r\n" +
            "VERSION:2.0\r\n" +
            "BEGIN:VEVENT\r\n" +
            "UID:timezone-test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            dtstart + "\r\n" +
            "DTEND:20261017T150000Z\r\n" +
            "END:VEVENT\r\n" +
            "END:VCALENDAR\r\n"
        );

        return calendar.Events.Single().Start;
    }

    private static readonly TimeZoneInfo Chicago =
        CalendarTimeZone.Resolve("America/Chicago");

    [Test]
    public void UtcEvent_ConvertsToPreviousChicagoDay()
    {
        var value = Start("DTSTART:20261016T010000Z");

        Assert.That(
            CalendarAppointmentTime.Convert(value, Chicago),
            Is.EqualTo(new DateTime(2026, 10, 15, 20, 0, 0))
        );
    }

    [Test]
    public void ExplicitTimezone_ConvertsCorrectly()
    {
        var value = Start(
            "DTSTART;TZID=America/New_York:20261016T090000"
        );

        Assert.That(
            CalendarAppointmentTime.Convert(value, Chicago),
            Is.EqualTo(new DateTime(2026, 10, 16, 8, 0, 0))
        );
    }

    [Test]
    public void AllDayEvent_PreservesDate()
    {
        var value = Start("DTSTART;VALUE=DATE:20261016");

        Assert.That(
            CalendarAppointmentTime.Convert(value, Chicago),
            Is.EqualTo(new DateTime(2026, 10, 16))
        );
    }

    [Test]
    public void FloatingEvent_PreservesWallClockTime()
    {
        var value = Start("DTSTART:20261016T090000");

        Assert.That(
            CalendarAppointmentTime.Convert(value, Chicago),
            Is.EqualTo(new DateTime(2026, 10, 16, 9, 0, 0))
        );
    }
}
