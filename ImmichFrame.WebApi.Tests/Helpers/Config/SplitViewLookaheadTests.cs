using ImmichFrame.WebApi.Models;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Helpers.Config;

[TestFixture]
public class SplitViewLookaheadTests
{
    [Test]
    public void DefaultLookahead_IsFour()
    {
        var settings = new GeneralSettings();

        Assert.That(settings.SplitViewLookahead, Is.EqualTo(4));
    }

    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(6)]
    [TestCase(8)]
    public void ValidLookahead_IsAccepted(int lookahead)
    {
        var settings = new GeneralSettings
        {
            SplitViewLookahead = lookahead
        };

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(9)]
    [TestCase(100)]
    public void InvalidLookahead_IsRejected(int lookahead)
    {
        var settings = new GeneralSettings
        {
            SplitViewLookahead = lookahead
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => settings.Validate()
        );
    }
}
