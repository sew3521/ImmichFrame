using Ical.Net;
using ImmichFrame.Core.Helpers;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarWindowTests
{
    private static Calendar LoadCalendar(string events)
    {
        return Calendar.Load(
            "BEGIN:VCALENDAR\r\n" +
            "VERSION:2.0\r\n" +
            "PRODID:-//ImmichFrame Tests//EN\r\n" +
            events +
            "END:VCALENDAR\r\n"
        );
    }

    [Test]
    public void PreviousEveningEvent_DoesNotAppearToday()
    {
        var calendar = LoadCalendar(
            "BEGIN:VEVENT\r\n" +
            "UID:previous-evening@test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            "DTSTART:20261016T010000Z\r\n" +
            "DTEND:20261016T030000Z\r\n" +
            "SUMMARY:Previous Evening\r\n" +
            "END:VEVENT\r\n"
        );

        var today = new DateTime(2026, 10, 16);
        var appointments = CalendarWindow.GetAppointments(calendar, today, 0);

        Assert.That(appointments, Is.Empty);
    }

    [Test]
    public void LookaheadTwoDays_IncludesThirdCalendarDay()
    {
        var calendar = LoadCalendar(
            "BEGIN:VEVENT\r\n" +
            "UID:future@test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            "DTSTART:20261017T150000Z\r\n" +
            "DTEND:20261017T160000Z\r\n" +
            "SUMMARY:Future Event\r\n" +
            "END:VEVENT\r\n"
        );

        var today = new DateTime(2026, 10, 16);
        var appointments = CalendarWindow.GetAppointments(calendar, today, 2);

        Assert.That(appointments, Has.Count.EqualTo(1));
    }

    [Test]
    public void InvalidLookahead_Throws()
    {
        var calendar = LoadCalendar("");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CalendarWindow.GetAppointments(
                calendar,
                new DateTime(2026, 10, 16),
                31
            )
        );
    }

    [Test]
    public void RecurringEvent_UsesOccurrenceDate()
    {
        var calendar = LoadCalendar(
            "BEGIN:VEVENT\r\n" +
            "UID:recurring@test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            "DTSTART:20261001T150000Z\r\n" +
            "DTEND:20261001T160000Z\r\n" +
            "RRULE:FREQ=DAILY;COUNT=30\r\n" +
            "SUMMARY:Recurring Event\r\n" +
            "END:VEVENT\r\n"
        );

        var today = new DateTime(2026, 10, 16);
        var appointments = CalendarWindow.GetAppointments(calendar, today, 0);

        Assert.That(appointments, Has.Count.EqualTo(1));
        Assert.That(appointments[0].StartTime.Date, Is.EqualTo(today));
        Assert.That(appointments[0].Summary, Is.EqualTo("Recurring Event"));
    }

    [Test]
    public void EventSpanningMidnight_RemainsVisible()
    {
        var calendar = LoadCalendar(
            "BEGIN:VEVENT\r\n" +
            "UID:overnight@test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            "DTSTART:20261016T043000Z\r\n" +
            "DTEND:20261016T063000Z\r\n" +
            "SUMMARY:Overnight Event\r\n" +
            "END:VEVENT\r\n"
        );

        var today = new DateTime(2026, 10, 16);
        var appointments = CalendarWindow.GetAppointments(calendar, today, 0);

        Assert.That(appointments, Has.Count.EqualTo(1));
        Assert.That(appointments[0].Summary, Is.EqualTo("Overnight Event"));
    }

    [Test]
    public void EventOutsideLookahead_IsExcluded()
    {
        var calendar = LoadCalendar(
            "BEGIN:VEVENT\r\n" +
            "UID:outside@test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            "DTSTART:20261019T150000Z\r\n" +
            "DTEND:20261019T160000Z\r\n" +
            "SUMMARY:Outside Window\r\n" +
            "END:VEVENT\r\n"
        );

        var today = new DateTime(2026, 10, 16);
        var appointments = CalendarWindow.GetAppointments(calendar, today, 2);

        Assert.That(appointments, Is.Empty);
    }

    [Test]
    public void CalendarSourceMetadata_IsAppliedToAppointments()
    {
        var calendar = LoadCalendar(
            "BEGIN:VEVENT\r\n" +
            "UID:source-metadata@test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            "DTSTART:20261016T150000Z\r\n" +
            "DTEND:20261016T160000Z\r\n" +
            "SUMMARY:Family Event\r\n" +
            "END:VEVENT\r\n"
        );

        var source = new ImmichFrame.Core.Models.CalendarSource
        {
            Id = Guid.NewGuid(),
            Name = "Family",
            Color = "#E69B69",
            Enabled = true
        };

        var appointments = CalendarWindow.GetAppointments(
            calendar,
            new DateTime(2026, 10, 16),
            0,
            source);

        Assert.That(appointments, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(appointments[0].CalendarId, Is.EqualTo(source.Id));
            Assert.That(appointments[0].CalendarName, Is.EqualTo("Family"));
            Assert.That(appointments[0].CalendarColor, Is.EqualTo("#E69B69"));
        });
    }


    [Test]
    public void MultiDayEvent_StartedBeforeWindow_RemainsVisible()
    {
        var calendar = LoadCalendar(
            "BEGIN:VEVENT\r\n" +
            "UID:on-call@test\r\n" +
            "DTSTAMP:20261001T120000Z\r\n" +
            "DTSTART;VALUE=DATE:20261013\r\n" +
            "DTEND;VALUE=DATE:20261021\r\n" +
            "SUMMARY:On Call\r\n" +
            "END:VEVENT\r\n"
        );

        var today = new DateTime(2026, 10, 17);

        var appointments = CalendarWindow.GetAppointments(
            calendar, today, 2);

        Assert.That(appointments, Has.Count.EqualTo(1));
        Assert.That(appointments[0].Summary, Is.EqualTo("On Call"));
    }

}
