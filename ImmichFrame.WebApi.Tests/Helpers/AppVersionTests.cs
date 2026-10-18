using ImmichFrame.WebApi.Helpers;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Helpers;

[TestFixture]
public class AppVersionTests
{
    [TestCase("2.0.0.0", "1.0.25.0", "2.0.0.0")]
    [TestCase(" 2.0.0.0 ", "1.0.25.0", "2.0.0.0")]
    [TestCase(null, "1.0.25.0", "1.0.25.0")]
    [TestCase("", "1.0.25.0", "1.0.25.0")]
    [TestCase(" ", "1.0.25.0", "1.0.25.0")]
    [TestCase(null, null, "unknown")]
    public void Resolve_ReturnsExpectedVersion(
        string? configured,
        string? assemblyVersion,
        string expected)
    {
        Assert.That(
            AppVersion.Resolve(configured, assemblyVersion),
            Is.EqualTo(expected)
        );
    }
}
