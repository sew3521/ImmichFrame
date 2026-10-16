using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Models;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarCacheKeyTests
{
    [Test]
    public void IdenticalConfiguration_ProducesSameKey()
    {
        var calendar = new CalendarSource
        {
            Id = Guid.NewGuid(),
            Name = "Family",
            Url = "https://example.com/family.ics",
            Color = "#6299EC",
            Enabled = true
        };

        var today = new DateTime(2026, 10, 16);

        var first = CalendarCacheKey.Create(today, 2, new[] { calendar });
        var second = CalendarCacheKey.Create(today, 2, new[] { calendar });

        Assert.That(first, Is.EqualTo(second));
    }

    [Test]
    public void ChangingCalendarColor_InvalidatesCache()
    {
        var calendar = new CalendarSource
        {
            Name = "Family",
            Url = "https://example.com/family.ics",
            Color = "#6299EC"
        };

        var today = new DateTime(2026, 10, 16);

        var before = CalendarCacheKey.Create(today, 2, new[] { calendar });

        calendar.Color = "#E69B69";

        var after = CalendarCacheKey.Create(today, 2, new[] { calendar });

        Assert.That(after, Is.Not.EqualTo(before));
    }

    [Test]
    public void ChangingEnabledState_InvalidatesCache()
    {
        var calendar = new CalendarSource
        {
            Url = "https://example.com/family.ics",
            Enabled = true
        };

        var today = new DateTime(2026, 10, 16);

        var before = CalendarCacheKey.Create(today, 2, new[] { calendar });

        calendar.Enabled = false;

        var after = CalendarCacheKey.Create(today, 2, new[] { calendar });

        Assert.That(after, Is.Not.EqualTo(before));
    }

    [Test]
    public void CacheKey_DoesNotExposeCalendarUrl()
    {
        var calendar = new CalendarSource
        {
            Url = "https://example.com/private-token-12345.ics"
        };

        var key = CalendarCacheKey.Create(
            new DateTime(2026, 10, 16),
            2,
            new[] { calendar });

        Assert.That(key, Does.Not.Contain("private-token"));
        Assert.That(key, Does.Not.Contain("example.com"));
    }
}
