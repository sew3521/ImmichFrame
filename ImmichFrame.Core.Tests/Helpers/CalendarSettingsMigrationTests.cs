using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Models;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarSettingsMigrationTests
{
    [Test]
    public void LegacyUrls_AreConvertedToNamedCalendars()
    {
        var legacy = new List<string>
        {
            "https://example.com/personal.ics",
            "https://example.com/family.ics"
        };

        var result = CalendarSettingsMigration.Resolve(null, legacy);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Calendar 1"));
        Assert.That(result[1].Name, Is.EqualTo("Calendar 2"));
        Assert.That(result[0].Url, Is.EqualTo(legacy[0]));
        Assert.That(result[1].Url, Is.EqualTo(legacy[1]));
        Assert.That(result.All(x => x.Enabled), Is.True);
        Assert.That(result.All(x => x.Id != Guid.Empty), Is.True);
        Assert.That(result[0].Color, Is.Not.EqualTo(result[1].Color));
    }

    [Test]
    public void EmptyStructuredList_DoesNotRestoreDeletedCalendars()
    {
        var result = CalendarSettingsMigration.Resolve(
            new List<CalendarSource>(),
            new List<string> { "https://example.com/old.ics" });

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ExistingCalendars_PreserveTheirProperties()
    {
        var id = Guid.NewGuid();

        var calendars = new List<CalendarSource>
        {
            new()
            {
                Id = id,
                Name = "Family",
                Url = "https://example.com/family.ics",
                Color = "#123456",
                Enabled = false
            }
        };

        var result = CalendarSettingsMigration.Resolve(
            calendars,
            new List<string> { "https://example.com/legacy.ics" });

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(id));
        Assert.That(result[0].Name, Is.EqualTo("Family"));
        Assert.That(result[0].Color, Is.EqualTo("#123456"));
        Assert.That(result[0].Enabled, Is.False);
    }

    [Test]
    public void DuplicateLegacyUrls_AreMigratedOnlyOnce()
    {
        var url = "https://example.com/calendar.ics";

        var result = CalendarSettingsMigration.Resolve(
            null,
            new List<string> { url, url, " ", url });

        Assert.That(result, Has.Count.EqualTo(1));
    }
}
