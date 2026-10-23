using ImmichFrame.WebApi.Models;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Helpers.Config;

[TestFixture]
public class CalendarDescriptionSettingsTests
{
    [Test]
    public void Defaults_EnableTruncationAt160Characters()
    {
        var settings = new GeneralSettings();

        Assert.Multiple(() =>
        {
            Assert.That(settings.TruncateCalendarDescriptions, Is.True);
            Assert.That(settings.CalendarDescriptionMaxCharacters, Is.EqualTo(160));
        });
    }

    [TestCase(40)]
    [TestCase(80)]
    [TestCase(160)]
    [TestCase(500)]
    public void ValidCharacterLimits_AreAccepted(int limit)
    {
        var settings = new GeneralSettings
        {
            CalendarDescriptionMaxCharacters = limit
        };

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(39)]
    [TestCase(501)]
    [TestCase(10000)]
    public void InvalidCharacterLimits_AreRejected(int limit)
    {
        var settings = new GeneralSettings
        {
            CalendarDescriptionMaxCharacters = limit
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => settings.Validate()
        );
    }

    [Test]
    public void DisabledTruncation_PreservesSetting()
    {
        var settings = new GeneralSettings
        {
            TruncateCalendarDescriptions = false,
            CalendarDescriptionMaxCharacters = 160
        };

        Assert.DoesNotThrow(() => settings.Validate());
        Assert.That(settings.TruncateCalendarDescriptions, Is.False);
    }
}
