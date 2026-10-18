using ImmichFrame.Core.Api;
using ImmichFrame.Core.Interfaces;

namespace ImmichFrame.Core.Helpers;

public static class AssetExclusionFilter
{
    public static bool HasExclusions(IAccountSettings settings)
    {
        return (settings.ExcludedPeople?.Count > 0)
            || (settings.ExcludedAlbums?.Count > 0);
    }

    public static async Task<bool> IsAllowed(
        AssetResponseDto asset,
        IAccountSettings settings,
        Func<Guid, Task<ICollection<AlbumResponseDto>>> getAlbums)
    {
        if (!HasExclusions(settings))
        {
            return true;
        }

        if (settings.ExcludedPeople is { Count: > 0 } excludedPeople)
        {
            // Missing people metadata must not bypass exclusions.
            if (asset.People == null)
            {
                return false;
            }

            if (asset.People.Any(person => excludedPeople.Contains(person.Id)))
            {
                return false;
            }
        }

        if (settings.ExcludedAlbums is { Count: > 0 } excludedAlbums)
        {
            var albums = await getAlbums(asset.Id);

            if (albums == null)
            {
                return false;
            }

            if (albums.Any(album => excludedAlbums.Contains(album.Id)))
            {
                return false;
            }
        }

        return true;
    }
}
