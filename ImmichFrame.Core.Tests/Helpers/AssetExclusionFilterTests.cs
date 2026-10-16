using ImmichFrame.Core.Api;
using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace ImmichFrame.Core.Tests.Helpers;

[TestFixture]
public class AssetExclusionFilterTests
{
    private static Mock<IAccountSettings> Settings(
        IEnumerable<Guid>? people = null,
        IEnumerable<Guid>? albums = null)
    {
        var settings = new Mock<IAccountSettings>();
        settings.SetupGet(x => x.ExcludedPeople)
            .Returns(people?.ToList() ?? new List<Guid>());
        settings.SetupGet(x => x.ExcludedAlbums)
            .Returns(albums?.ToList() ?? new List<Guid>());
        return settings;
    }

    [Test]
    public async Task NoExclusions_AllowsAsset()
    {
        var asset = new AssetResponseDto { Id = Guid.NewGuid() };
        var allowed = await AssetExclusionFilter.IsAllowed(
            asset, Settings().Object,
            _ => Task.FromResult<ICollection<AlbumResponseDto>>(
                new List<AlbumResponseDto>()));

        Assert.That(allowed, Is.True);
    }

    [Test]
    public async Task ExcludedPerson_RejectsAsset()
    {
        var personId = Guid.NewGuid();
        var asset = new AssetResponseDto
        {
            Id = Guid.NewGuid(),
            People = new List<PersonResponseDto>
            {
                new PersonResponseDto { Id = personId }
            }
        };

        var allowed = await AssetExclusionFilter.IsAllowed(
            asset, Settings(people: new[] { personId }).Object,
            _ => Task.FromResult<ICollection<AlbumResponseDto>>(
                new List<AlbumResponseDto>()));

        Assert.That(allowed, Is.False);
    }

    [Test]
    public async Task ExcludedAlbum_RejectsAsset()
    {
        var albumId = Guid.NewGuid();
        var asset = new AssetResponseDto { Id = Guid.NewGuid() };

        var allowed = await AssetExclusionFilter.IsAllowed(
            asset, Settings(albums: new[] { albumId }).Object,
            _ => Task.FromResult<ICollection<AlbumResponseDto>>(
                new List<AlbumResponseDto>
                {
                    new AlbumResponseDto { Id = albumId }
                }));

        Assert.That(allowed, Is.False);
    }

    [Test]
    public async Task MissingPeopleMetadata_RejectsAsset()
    {
        var asset = new AssetResponseDto { Id = Guid.NewGuid() };

        var allowed = await AssetExclusionFilter.IsAllowed(
            asset, Settings(people: new[] { Guid.NewGuid() }).Object,
            _ => Task.FromResult<ICollection<AlbumResponseDto>>(
                new List<AlbumResponseDto>()));

        Assert.That(allowed, Is.False);
    }

    [Test]
    public async Task IncludedAndExcludedPerson_RejectsAsset()
    {
        var includedId = Guid.NewGuid();
        var excludedId = Guid.NewGuid();

        var asset = new AssetResponseDto
        {
            Id = Guid.NewGuid(),
            People = new List<PersonResponseDto>
            {
                new PersonResponseDto { Id = includedId },
                new PersonResponseDto { Id = excludedId }
            }
        };

        var allowed = await AssetExclusionFilter.IsAllowed(
            asset,
            Settings(people: new[] { excludedId }).Object,
            _ => Task.FromResult<ICollection<AlbumResponseDto>>(
                new List<AlbumResponseDto>()));

        Assert.That(allowed, Is.False);
    }

    [Test]
    public void AlbumLookupFailure_DoesNotAllowAsset()
    {
        var asset = new AssetResponseDto { Id = Guid.NewGuid() };

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await AssetExclusionFilter.IsAllowed(
                asset,
                Settings(albums: new[] { Guid.NewGuid() }).Object,
                _ => throw new InvalidOperationException("Album lookup failed")
            )
        );
    }

}
