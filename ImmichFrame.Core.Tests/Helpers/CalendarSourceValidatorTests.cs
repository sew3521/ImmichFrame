using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Models;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarSourceValidatorTests
{
    private static CalendarSource Valid() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Family",
        Url = "https://example.com/family.ics",
        Color = "#6299EC",
        Enabled = true
    };

    [Test]
    public void ValidCalendar_IsAccepted()
    {
        Assert.DoesNotThrow(() =>
            CalendarSourceValidator.Validate(new[] { Valid() }));
    }

    [Test]
    public void EmptyList_IsAccepted()
    {
        Assert.DoesNotThrow(() =>
            CalendarSourceValidator.Validate(
                Array.Empty<CalendarSource>()));
    }

    [Test]
    public void DuplicateIds_AreRejected()
    {
        var first = Valid();
        var second = Valid();
        second.Id = first.Id;

        Assert.Throws<ArgumentException>(() =>
            CalendarSourceValidator.Validate(
                new[] { first, second }));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void EmptyName_IsRejected(string name)
    {
        var calendar = Valid();
        calendar.Name = name;

        Assert.Throws<ArgumentException>(() =>
            CalendarSourceValidator.Validate(new[] { calendar }));
    }

    [TestCase("")]
    [TestCase("not-a-url")]
    [TestCase("file:///etc/passwd")]
    public void InvalidUrl_IsRejected(string url)
    {
        var calendar = Valid();
        calendar.Url = url;

        Assert.Throws<ArgumentException>(() =>
            CalendarSourceValidator.Validate(new[] { calendar }));
    }

    [TestCase("#123")]
    [TestCase("red")]
    [TestCase("#GGGGGG")]
    public void InvalidColor_IsRejected(string color)
    {
        var calendar = Valid();
        calendar.Color = color;

        Assert.Throws<ArgumentException>(() =>
            CalendarSourceValidator.Validate(new[] { calendar }));
    }

    [Test]
    public void WebcalUrl_IsAccepted()
    {
        var calendar = Valid();
        calendar.Url = "webcal://example.com/family.ics";

        Assert.DoesNotThrow(() =>
            CalendarSourceValidator.Validate(new[] { calendar }));
    }

    [Test]
    public void DisabledCalendar_StillRequiresValidConfiguration()
    {
        var calendar = Valid();
        calendar.Enabled = false;
        calendar.Url = "";

        Assert.Throws<ArgumentException>(() =>
            CalendarSourceValidator.Validate(new[] { calendar }));
    }
}
