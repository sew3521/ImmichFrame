using ImmichFrame.WebApi.Models;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Helpers.Config;

[TestFixture]
public class TimeZoneSettingsTests
{
    [Test]
    public void DefaultTimezone_UsesContainer()
    {
        var settings = new GeneralSettings();

        Assert.That(settings.TimeZoneId, Is.Empty);
        Assert.DoesNotThrow(() => settings.Validate());
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("America/Chicago")]
    [TestCase("America/New_York")]
    [TestCase("America/Los_Angeles")]
    [TestCase("UTC")]
    public void ValidTimezone_IsAccepted(string timezone)
    {
        var settings = new GeneralSettings
        {
            TimeZoneId = timezone
        };

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [TestCase("Not/A_Real_Timezone")]
    [TestCase("Mars/Olympus_Mons")]
    public void InvalidTimezone_IsRejected(string timezone)
    {
        var settings = new GeneralSettings
        {
            TimeZoneId = timezone
        };

        Assert.Throws<ArgumentException>(
            () => settings.Validate()
        );
    }
}
