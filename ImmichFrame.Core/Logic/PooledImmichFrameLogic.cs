using ImmichFrame.Core.Api;
using ImmichFrame.Core.Exceptions;
using ImmichFrame.Core.Helpers;
using ImmichFrame.Core.Interfaces;
using ImmichFrame.Core.Logic.Pool;
using ImmichFrame.Core.Models;
using Microsoft.Extensions.Logging;

namespace ImmichFrame.Core.Logic;

public class PooledImmichFrameLogic : IAccountImmichFrameLogic, IDisposable
{
    private readonly ILogger<PooledImmichFrameLogic> _logger;
    private readonly IGeneralSettings _generalSettings;
    private readonly IApiCache _apiCache;
    private readonly IAssetPool _pool;
    private readonly ImmichApi _immichApi;
    private readonly string _downloadLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageCache");

    public PooledImmichFrameLogic(IAccountSettings accountSettings, IGeneralSettings generalSettings, IHttpClientFactory httpClientFactory, ILogger<PooledImmichFrameLogic> logger)
    {
        _logger = logger;
        _generalSettings = generalSettings;

        var httpClient = httpClientFactory.CreateClient(ImmichApiHttpClientExtensions.ImmichApiAccountClient);
        AccountSettings = accountSettings;

        httpClient.UseApiKey(accountSettings.ApiKey);
        _immichApi = new ImmichApi(accountSettings.ImmichServerUrl, httpClient);

        _apiCache = new ApiCache(RefreshInterval(generalSettings.RefreshAlbumPeopleInterval));
        _pool = BuildPool(accountSettings);
    }

    private static TimeSpan RefreshInterval(int hours)
        => hours > 0 ? TimeSpan.FromHours(hours) : TimeSpan.FromMilliseconds(1);

    public IAccountSettings AccountSettings { get; }

    private IAssetPool BuildPool(IAccountSettings accountSettings)
    {
        var hasSource = accountSettings.ShowFavorites
            || (accountSettings.Albums?.Any() ?? false)
            || (accountSettings.People?.Any() ?? false)
            || (accountSettings.Tags?.Any() ?? false);

        var pools = new List<IAssetPool>();

        // Memories-only skips the library search. Every other case needs it.
        if (!accountSettings.ShowMemories || hasSource)
        {
            pools.Add(new AccountSearchPool(_apiCache, _immichApi, accountSettings));
        }

        if (accountSettings.ShowMemories)
        {
            pools.Add(new MemoryAssetsPool(_immichApi, accountSettings));
        }

        return pools.Count == 1 ? pools[0] : new MultiAssetPool(pools);
    }

    private async Task<bool> IsAssetAllowed(
        AssetResponseDto asset,
        bool refreshMetadata = false)
    {
        if (!AssetExclusionFilter.HasExclusions(AccountSettings))
        {
            return true;
        }

        try
        {
            if (refreshMetadata ||
                (AccountSettings.ExcludedPeople.Count > 0 && asset.People == null))
            {
                asset = await _immichApi.GetAssetInfoAsync(asset.Id, null, null);

                if (asset == null)
                {
                    return false;
                }
            }

            return await AssetExclusionFilter.IsAllowed(
                asset,
                AccountSettings,
                async id => await _immichApi.GetAllAlbumsAsync(
                    id, null, null, null, null)
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Asset exclusion verification failed for asset {AssetId}. Asset rejected.",
                asset.Id
            );

            // Fail closed: never approve an asset we cannot verify.
            return false;
        }
    }

    public async Task<AssetResponseDto?> GetNextAsset()
    {
        if (!AssetExclusionFilter.HasExclusions(AccountSettings))
        {
            return (await _pool.GetAssets(1)).FirstOrDefault();
        }

        var selector = new FilteredAssetSelector(_pool, asset => IsAssetAllowed(asset));
        return await selector.GetNextAsset();
    }

    public async Task<IEnumerable<AssetResponseDto>> GetAssets()
    {
        if (!AssetExclusionFilter.HasExclusions(AccountSettings))
        {
            return await _pool.GetAssets(25);
        }

        var selector = new FilteredAssetSelector(_pool, asset => IsAssetAllowed(asset));
        return await selector.GetAssets();
    }

    public async Task<AssetResponseDto> GetAssetInfoById(Guid assetId)
    {
        AssetResponseDto? asset;

        try
        {
            asset = await _immichApi.GetAssetInfoAsync(assetId, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Unable to retrieve metadata for asset {AssetId}.",
                assetId
            );

            throw new AssetNotFoundException(
                $"Asset '{assetId}' could not be verified.");
        }

        if (asset == null || !await IsAssetAllowed(asset))
        {
            throw new AssetNotFoundException(
                $"Asset '{assetId}' is excluded or could not be verified.");
        }

        return asset;
    }

    public async Task<IEnumerable<AssetFaceResponseDto>> GetAssetFacesById(Guid assetId) => await _immichApi.GetFacesAsync(assetId);

    public async Task<IEnumerable<AlbumResponseDto>> GetAlbumInfoById(Guid assetId) => await _immichApi.GetAllAlbumsAsync(assetId, null, null, null, null);

    public async Task<long> GetTotalAssets() => await _pool.GetAssetCount();

    public async Task<AssetResponse> GetAsset(Guid id, AssetTypeEnum? assetType = null, string? rangeHeader = null)
    {
        AssetResponseDto? assetInfo = null;

        if (AssetExclusionFilter.HasExclusions(AccountSettings) || !assetType.HasValue)
        {
            try
            {
                assetInfo = await _immichApi.GetAssetInfoAsync(id, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Unable to retrieve metadata for asset {AssetId}. Asset rejected.",
                    id
                );

                throw new AssetNotFoundException(
                    $"Asset '{id}' could not be verified.");
            }

            if (assetInfo == null)
                throw new AssetNotFoundException($"Asset '{id}' was not found!");

            if (!await IsAssetAllowed(assetInfo))
                throw new AssetNotFoundException(
                    $"Asset '{id}' is excluded or could not be verified.");

            assetType = assetInfo.Type;
        }

        if (assetType == AssetTypeEnum.IMAGE)
        {
            var (fileName, contentType, fileStream) = await GetImageAsset(id);
            return new AssetResponse
            {
                FileName = fileName,
                ContentType = contentType,
                FileStream = fileStream,
                ContentRange = null,
                IsPartial = false,
                Owner = null,
                ContentLength = null
            };
        }

        if (assetType == AssetTypeEnum.VIDEO)
        {
            return await GetVideoAsset(id, rangeHeader);
        }

        throw new AssetNotFoundException($"Asset {id} is not a supported media type ({assetType}).");
    }
    private async Task<(string fileName, string ContentType, Stream fileStream)> GetImageAsset(Guid id)
    {
        if (_generalSettings.DownloadImages)
        {
            if (!Directory.Exists(_downloadLocation))
            {
                Directory.CreateDirectory(_downloadLocation);
            }

            var file = Directory.GetFiles(_downloadLocation)
                .FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == id.ToString());

            if (!string.IsNullOrWhiteSpace(file))
            {
                if (_generalSettings.RenewImagesDuration > (DateTime.UtcNow - File.GetCreationTimeUtc(file)).Days)
                {
                    var fs = File.OpenRead(file);

                    var ex = Path.GetExtension(file).TrimStart('.');

                    return (Path.GetFileName(file), $"image/{ex}", fs);
                }

                File.Delete(file);
            }
        }

        var data = await _immichApi.ViewAssetAsync(null, id, string.Empty, AssetMediaSize.Preview, null);

        if (data == null)
            throw new AssetNotFoundException($"Asset {id} was not found!");

        var contentType = "";
        if (data.Headers.ContainsKey("Content-Type"))
        {
            contentType = data.Headers["Content-Type"].FirstOrDefault() ?? "";
        }

        var ext = contentType.ToLower() == "image/webp" ? "webp" : "jpeg";
        var fileName = $"{id}.{ext}";

        if (_generalSettings.DownloadImages)
        {
            var stream = data.Stream;

            var filePath = Path.Combine(_downloadLocation, fileName);

            // save to folder
            var fs = File.Create(filePath);
            await stream.CopyToAsync(fs);
            fs.Position = 0;
            return (Path.GetFileName(filePath), contentType, fs);
        }

        return (fileName, contentType, data.Stream);
    }

    private async Task<AssetResponse> GetVideoAsset(Guid id, string? rangeHeader = null)
    {
        var videoResponse = string.IsNullOrEmpty(rangeHeader)
            ? await _immichApi.PlayAssetVideoAsync(id, null, null)
            : await _immichApi.PlayAssetVideoWithRangeAsync(id, rangeHeader);

        var contentType = videoResponse.Headers.TryGetValue("Content-Type", out var ct)
            ? ct.FirstOrDefault() ?? "video/mp4"
            : "video/mp4";

        var contentRange = videoResponse.Headers.TryGetValue("Content-Range", out var cr)
            ? cr.FirstOrDefault()
            : null;

        long? contentLength = videoResponse.Headers.TryGetValue("Content-Length", out var cl)
            && long.TryParse(cl.FirstOrDefault(), out var clValue) ? clValue : null;

        return new AssetResponse
        {
            FileName = $"{id}.mp4",
            ContentType = contentType,
            FileStream = videoResponse.Stream,
            ContentRange = contentRange,
            IsPartial = videoResponse.StatusCode == 206,
            Owner = videoResponse,
            ContentLength = contentLength
        };
    }
    public async Task SendWebhookNotification(IWebhookNotification notification) =>
        await WebhookHelper.SendWebhookNotification(notification, _generalSettings.Webhook);

    public override string ToString() => $"Account Pool [{_immichApi.BaseUrl}]";

    public void Dispose() => (_apiCache as IDisposable)?.Dispose();
}
