using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Interfaces;
using ImmichFrame.Core.Models;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class CalendarFallbackCacheTests
{
    private static CalendarSource Source() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Family",
        Url = "https://example.com/family.ics"
    };

    private static IAppointment Event(string title) => new Appointment
    {
        Summary = title,
        StartTime = new DateTime(2026, 10, 18, 9, 0, 0),
        EndTime = new DateTime(2026, 10, 18, 10, 0, 0)
    };

    [Test]
    public void SuccessfulData_IsAvailableDuringFailure()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);

        var result = cache.GetFallback(
            source, today, 2, now.AddMinutes(30));

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result![0].Summary, Is.EqualTo("Dinner"));
    }

    [Test]
    public void ExpiredData_IsNotReturned()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);

        var result = cache.GetFallback(
            source, today, 2, now.AddHours(3));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void UrlChange_InvalidatesPreviousData()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);

        source.Url = "https://example.com/another.ics";

        Assert.That(
            cache.GetFallback(source, today, 2, now.AddMinutes(5)),
            Is.Null);
    }

    [Test]
    public void ColorChange_InvalidatesPreviousData()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);

        source.Color = "#E69B69";

        Assert.That(
            cache.GetFallback(source, today, 2, now.AddMinutes(5)),
            Is.Null);
    }

    [Test]
    public void NameChange_InvalidatesPreviousData()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);

        source.Name = "Updated Family Calendar";

        Assert.That(
            cache.GetFallback(source, today, 2, now.AddMinutes(5)),
            Is.Null);
    }

    [Test]
    public void ChangedWindow_DoesNotReuseOldAppointments()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);

        Assert.That(
            cache.GetFallback(source, today.AddDays(1), 2, now),
            Is.Null);
    }

    [Test]
    public void SuccessfulEmptyResult_ReplacesOldEvents()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);
        cache.Store(source, today, 2, Array.Empty<IAppointment>(),
            now.AddMinutes(15));

        var result = cache.GetFallback(
            source, today, 2, now.AddMinutes(20));

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void TimezoneChange_InvalidatesFallback()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(
            source, today, 2,
            new[] { Event("Dinner") },
            now, "America/Chicago");

        Assert.That(
            cache.GetFallback(
                source, today, 2,
                now.AddMinutes(5), "America/Chicago"),
            Has.Count.EqualTo(1));

        Assert.That(
            cache.GetFallback(
                source, today, 2,
                now.AddMinutes(5), "UTC"),
            Is.Null);
    }

    [Test]
    public void RemovedCalendar_IsDiscarded()
    {
        var cache = new CalendarFallbackCache();
        var source = Source();
        var today = new DateTime(2026, 10, 18);
        var now = DateTimeOffset.UtcNow;

        cache.Store(source, today, 2, new[] { Event("Dinner") }, now);
        cache.RetainOnly(Array.Empty<CalendarSource>());

        Assert.That(
            cache.GetFallback(source, today, 2, now),
            Is.Null);
    }
}
