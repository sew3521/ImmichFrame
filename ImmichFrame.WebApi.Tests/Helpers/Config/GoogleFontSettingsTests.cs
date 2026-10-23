using ImmichFrame.WebApi.Models;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Helpers.Config;

[TestFixture]
public class GoogleFontSettingsTests
{
    [Test]
    public void Defaults_DisableGoogleFontsAndSelectInter()
    {
        var settings = new GeneralSettings();

        Assert.Multiple(() =>
        {
            Assert.That(settings.UseGoogleFont, Is.False);
            Assert.That(settings.GoogleFontFamily, Is.EqualTo("Inter"));
        });
    }

    [TestCase("Inter")]
    [TestCase("Open Sans")]
    [TestCase("Roboto Slab")]
    [TestCase("Bebas Neue")]
    [TestCase("DM Sans")]
    public void ValidFontNames_AreAccepted(string family)
    {
        var settings = new GeneralSettings
        {
            UseGoogleFont = true,
            GoogleFontFamily = family
        };

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("https://fonts.googleapis.com")]
    [TestCase("Inter; color: red")]
    [TestCase("Inter{background:red}")]
    [TestCase("Inter\nRoboto")]
    public void InvalidFontNames_AreRejected(string family)
    {
        var settings = new GeneralSettings
        {
            UseGoogleFont = true,
            GoogleFontFamily = family
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }

    [Test]
    public void DisabledGoogleFonts_DoNotRequireValidFamily()
    {
        var settings = new GeneralSettings
        {
            UseGoogleFont = false,
            GoogleFontFamily = "invalid;css"
        };

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [Test]
    public void ExcessivelyLongFontName_IsRejected()
    {
        var settings = new GeneralSettings
        {
            UseGoogleFont = true,
            GoogleFontFamily = new string('A', 101)
        };

        Assert.Throws<ArgumentException>(() => settings.Validate());
    }
}
