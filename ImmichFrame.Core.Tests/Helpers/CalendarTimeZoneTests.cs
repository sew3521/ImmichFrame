using ImmichFrame.Core.Helpers;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarTimeZoneTests
{
    [Test]
    public void EmptyTimezone_UsesContainerTimezone()
    {
        Assert.That(
            CalendarTimeZone.Resolve(""),
            Is.EqualTo(TimeZoneInfo.Local)
        );
    }

    [Test]
    public void ExplicitTimezone_IsResolved()
    {
        Assert.That(
            CalendarTimeZone.Resolve("America/Chicago").Id,
            Is.EqualTo("America/Chicago")
        );
    }

    [Test]
    public void ChicagoDate_DiffersFromUtcNearMidnight()
    {
        var instant = new DateTimeOffset(
            2026, 10, 16, 2, 0, 0, TimeSpan.Zero
        );

        var chicago = CalendarTimeZone.Resolve("America/Chicago");
        var utc = CalendarTimeZone.Resolve("UTC");

        Assert.That(
            CalendarTimeZone.Today(chicago, instant),
            Is.EqualTo(new DateTime(2026, 10, 15))
        );

        Assert.That(
            CalendarTimeZone.Today(utc, instant),
            Is.EqualTo(new DateTime(2026, 10, 16))
        );
    }

    [Test]
    public void DaylightSavingTime_IsApplied()
    {
        var chicago = CalendarTimeZone.Resolve("America/Chicago");

        var winter = new DateTimeOffset(
            2026, 1, 15, 18, 0, 0, TimeSpan.Zero
        );

        var summer = new DateTimeOffset(
            2026, 7, 15, 18, 0, 0, TimeSpan.Zero
        );

        Assert.That(
            CalendarTimeZone.ToLocal(winter, chicago).Hour,
            Is.EqualTo(12)
        );

        Assert.That(
            CalendarTimeZone.ToLocal(summer, chicago).Hour,
            Is.EqualTo(13)
        );
    }

    [Test]
    public void InvalidTimezone_Throws()
    {
        Assert.Throws<TimeZoneNotFoundException>(() =>
            CalendarTimeZone.Resolve("Mars/Olympus_Mons")
        );
    }
}
