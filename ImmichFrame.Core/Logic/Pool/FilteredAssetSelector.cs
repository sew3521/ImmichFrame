using ImmichFrame.Core.Api;

namespace ImmichFrame.Core.Logic.Pool;

public class FilteredAssetSelector(
    IAssetPool pool,
    Func<AssetResponseDto, Task<bool>> isAllowed)
{
    public async Task<AssetResponseDto?> GetNextAsset()
    {
        var seen = new HashSet<Guid>();

        for (var attempt = 0; attempt < 20; attempt++)
        {
            var candidates = await pool.GetAssets(1);
            var candidate = candidates.FirstOrDefault();

            if (candidate == null)
            {
                return null;
            }

            if (!seen.Add(candidate.Id))
            {
                continue;
            }

            if (await isAllowed(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    public async Task<IEnumerable<AssetResponseDto>> GetAssets(
        int targetCount = 25)
    {
        var approved = new List<AssetResponseDto>();
        var seen = new HashSet<Guid>();

        for (var attempt = 0;
             attempt < 5 && approved.Count < targetCount;
             attempt++)
        {
            var candidates = (await pool.GetAssets(targetCount)).ToList();

            if (candidates.Count == 0)
            {
                break;
            }

            foreach (var candidate in candidates)
            {
                if (approved.Count >= targetCount)
                {
                    break;
                }

                if (!seen.Add(candidate.Id))
                {
                    continue;
                }

                if (await isAllowed(candidate))
                {
                    approved.Add(candidate);
                }
            }
        }

        return approved;
    }
}
