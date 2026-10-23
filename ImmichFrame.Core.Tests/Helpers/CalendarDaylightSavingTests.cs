using Ical.Net;
using ImmichFrame.Core.Helpers;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarDaylightSavingTests
{
    private static readonly TimeZoneInfo Chicago =
        CalendarTimeZone.Resolve("America/Chicago");

    private static Calendar LoadEvent(
        string start,
        string end)
    {
        return Calendar.Load(
            "BEGIN:VCALENDAR\r\n" +
            "VERSION:2.0\r\n" +
            "BEGIN:VEVENT\r\n" +
            "UID:dst-test\r\n" +
            "DTSTAMP:20260101T120000Z\r\n" +
            $"DTSTART:{start}\r\n" +
            $"DTEND:{end}\r\n" +
            "SUMMARY:DST Test\r\n" +
            "END:VEVENT\r\n" +
            "END:VCALENDAR\r\n"
        );
    }

    [Test]
    public void SpringForward_EventAfterClockChange()
    {
        var calendar = LoadEvent(
            "20260308T080000Z",
            "20260308T090000Z"
        );

        var appointments = CalendarWindow.GetAppointments(
            calendar,
            new DateTime(2026, 3, 8),
            0,
            Chicago
        );

        Assert.That(appointments, Has.Count.EqualTo(1));
        Assert.That(
            appointments[0].StartTime,
            Is.EqualTo(new DateTime(2026, 3, 8, 3, 0, 0))
        );
    }

    [Test]
    public void FallBack_EventAfterClockChange()
    {
        var calendar = LoadEvent(
            "20261101T080000Z",
            "20261101T090000Z"
        );

        var appointments = CalendarWindow.GetAppointments(
            calendar,
            new DateTime(2026, 11, 1),
            0,
            Chicago
        );

        Assert.That(appointments, Has.Count.EqualTo(1));
        Assert.That(
            appointments[0].StartTime,
            Is.EqualTo(new DateTime(2026, 11, 1, 2, 0, 0))
        );
    }

    [Test]
    public void SpringForward_PreviousEveningExcluded()
    {
        var calendar = LoadEvent(
            "20260308T040000Z",
            "20260308T050000Z"
        );

        var appointments = CalendarWindow.GetAppointments(
            calendar,
            new DateTime(2026, 3, 8),
            0,
            Chicago
        );

        Assert.That(appointments, Is.Empty);
    }

    [Test]
    public void FallBack_PreviousEveningExcluded()
    {
        var calendar = LoadEvent(
            "20261101T030000Z",
            "20261101T040000Z"
        );

        var appointments = CalendarWindow.GetAppointments(
            calendar,
            new DateTime(2026, 11, 1),
            0,
            Chicago
        );

        Assert.That(appointments, Is.Empty);
    }
}
