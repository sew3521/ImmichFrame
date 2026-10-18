using ImmichFrame.Core.Api;
using ImmichFrame.Core.Logic.Pool;
using Moq;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Logic.Pool;

[TestFixture]
public class FilteredAssetSelectorTests
{
    private static AssetResponseDto Asset() =>
        new() { Id = Guid.NewGuid() };

    [Test]
    public async Task GetNextAsset_SkipsExcludedCandidate()
    {
        var excluded = Asset();
        var allowed = Asset();

        var pool = new Mock<IAssetPool>();
        var queue = new Queue<AssetResponseDto>(
            new[] { excluded, allowed });

        pool.Setup(x => x.GetAssets(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
                queue.Count > 0
                    ? new[] { queue.Dequeue() }
                    : Array.Empty<AssetResponseDto>());

        var selector = new FilteredAssetSelector(
            pool.Object,
            asset => Task.FromResult(asset.Id != excluded.Id));

        var result = await selector.GetNextAsset();

        Assert.That(result?.Id, Is.EqualTo(allowed.Id));
    }

    [Test]
    public async Task GetAssets_RemovesExcludedAndDuplicateAssets()
    {
        var excluded = Asset();
        var allowed = Asset();

        var pool = new Mock<IAssetPool>();

        pool.Setup(x => x.GetAssets(25, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { excluded, allowed, allowed });

        var selector = new FilteredAssetSelector(
            pool.Object,
            asset => Task.FromResult(asset.Id != excluded.Id));

        var results = (await selector.GetAssets()).ToList();

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].Id, Is.EqualTo(allowed.Id));
    }

    [Test]
    public async Task GetNextAsset_DoesNotRevalidateDuplicate()
    {
        var excluded = Asset();
        var pool = new Mock<IAssetPool>();

        pool.Setup(x => x.GetAssets(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { excluded });

        var validationCount = 0;

        var selector = new FilteredAssetSelector(
            pool.Object,
            asset =>
            {
                validationCount++;
                return Task.FromResult(false);
            });

        var result = await selector.GetNextAsset();

        Assert.That(result, Is.Null);
        Assert.That(validationCount, Is.EqualTo(1));
    }
}
