using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Interfaces;
using ImmichFrame.Core.Models;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarEventDeduplicatorTests
{
    private static IAppointment Event(
        string title,
        int startHour = 9,
        int endHour = 10,
        string color = "#6299EC")
    {
        return new Appointment
        {
            Summary = title,
            StartTime = new DateTime(2026, 10, 18, startHour, 0, 0),
            EndTime = new DateTime(2026, 10, 18, endHour, 0, 0),
            CalendarColor = color
        };
    }

    [Test]
    public void IdenticalEvents_AreDeduplicated()
    {
        var result = CalendarEventDeduplicator.Deduplicate(new[]
        {
            Event("Family Dinner"),
            Event("Family Dinner", color: "#E69B69")
        });

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].CalendarColor, Is.EqualTo("#6299EC"));
    }

    [Test]
    public void DifferentTimes_ArePreserved()
    {
        var result = CalendarEventDeduplicator.Deduplicate(new[]
        {
            Event("Meeting", 9, 10),
            Event("Meeting", 10, 11)
        });

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void DifferentTitles_ArePreserved()
    {
        var result = CalendarEventDeduplicator.Deduplicate(new[]
        {
            Event("Meeting"),
            Event("Dinner")
        });

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void Titles_AreCaseAndWhitespaceInsensitive()
    {
        var result = CalendarEventDeduplicator.Deduplicate(new[]
        {
            Event(" Family Dinner "),
            Event("family dinner")
        });

        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public void DifferentEndTimes_ArePreserved()
    {
        var result = CalendarEventDeduplicator.Deduplicate(new[]
        {
            Event("Meeting", 9, 10),
            Event("Meeting", 9, 11)
        });

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void DifferentOccurrenceDates_ArePreserved()
    {
        var first = Event("Recurring Meeting");
        var second = Event("Recurring Meeting");

        second.StartTime = second.StartTime.AddDays(1);
        second.EndTime = second.EndTime.AddDays(1);

        var result = CalendarEventDeduplicator.Deduplicate(
            new[] { first, second });

        Assert.That(result, Has.Count.EqualTo(2));
    }
}
