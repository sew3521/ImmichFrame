using ImmichFrame.WebApi.Models;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Helpers.Config;

[TestFixture]
public class FrostedGlassOpacityTests
{
    [TestCase(10)]
    [TestCase(45)]
    [TestCase(95)]
    public void ValidOpacity_IsAccepted(int opacity)
    {
        var settings = new GeneralSettings
        {
            FrostedGlassOpacity = opacity
        };

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [Test]
    public void NullOpacity_IsAcceptedForLegacySettings()
    {
        var settings = new GeneralSettings
        {
            FrostedGlassOpacity = null
        };

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [TestCase(0)]
    [TestCase(9)]
    [TestCase(96)]
    [TestCase(100)]
    [TestCase(-1)]
    public void InvalidOpacity_IsRejected(int opacity)
    {
        var settings = new GeneralSettings
        {
            FrostedGlassOpacity = opacity
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => settings.Validate());
    }
}
