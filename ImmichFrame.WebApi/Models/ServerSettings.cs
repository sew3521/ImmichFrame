using System.Text.Json.Serialization;
using ImmichFrame.Core.Interfaces;
using YamlDotNet.Serialization;

namespace ImmichFrame.WebApi.Models;

public class ServerSettings : IServerSettings
{
    [YamlMember(Alias = "General")]
    [JsonPropertyName("General")]
    public GeneralSettings? GeneralSettingsImpl { get; set; }

    [YamlMember(Alias = "Accounts")]
    [JsonPropertyName("Accounts")]
    public IEnumerable<ServerAccountSettings> AccountsImpl { get; set; }

    //Covariance not allowed on interface impls
    [JsonIgnore]
    [YamlIgnore]
    public IGeneralSettings GeneralSettings => GeneralSettingsImpl ?? new GeneralSettings();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<IAccountSettings> Accounts => AccountsImpl;

    public void Validate()
    {
        GeneralSettings.Validate();

        foreach (var account in Accounts)
        {
            account.ValidateAndInitialize();
        }
    }
}

public class GeneralSettings : IGeneralSettings
{
    public bool DownloadImages { get; set; } = false;
    public string Language { get; set; } = "en";
    public string? ImageLocationFormat { get; set; } = "City,State,Country";
    public string? PhotoDateFormat { get; set; } = "MM/dd/yyyy";
    public int Interval { get; set; } = 45;
    public double TransitionDuration { get; set; } = 1;
    public bool ShowClock { get; set; } = true;
    public bool EnableFrostedGlassUI { get; set; } = false;
    public int? FrostedGlassOpacity { get; set; } = 45;
    public string? ClockFormat { get; set; } = "hh:mm";
    public string? ClockDateFormat { get; set; } = "eee, MMM d";
    public bool ShowProgressBar { get; set; } = true;
    public bool ShowPhotoDate { get; set; } = true;
    public bool ShowImageDesc { get; set; } = true;
    public bool ShowPeopleDesc { get; set; } = true;
    public bool ShowPeopleAge { get; set; } = false;
    public bool ShowTagsDesc { get; set; } = true;
    public bool ShowAlbumName { get; set; } = true;
    public bool ShowImageLocation { get; set; } = true;
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string Style { get; set; } = "none";
    public string? BaseFontSize { get; set; }
    public int SplitViewLookahead { get; set; } = 4;
    public bool UseGoogleFont { get; set; } = false;
    public string GoogleFontFamily { get; set; } = "Inter";
    public bool ShowWeatherDescription { get; set; } = true;
    public string? WeatherIconUrl { get; set; } = "https://openweathermap.org/img/wn/{IconId}.png";
    public bool ImageZoom { get; set; } = true;
    public bool ImagePan { get; set; } = false;
    public bool ImageFill { get; set; } = false;
    public bool PlayAudio { get; set; } = false;
    public string Layout { get; set; } = "splitview";
    public int RenewImagesDuration { get; set; } = 30;
    public List<string> Webcalendars { get; set; } = new();
    public List<ImmichFrame.Core.Models.CalendarSource>? Calendars { get; set; }
    public int CalendarDaysAhead { get; set; } = 0;
    public string TimeZoneId { get; set; } = "";
    public bool TruncateCalendarDescriptions { get; set; } = true;
    public int CalendarDescriptionMaxCharacters { get; set; } = 160;
    public int RefreshAlbumPeopleInterval { get; set; } = 12;
    public string? WeatherApiKey { get; set; } = string.Empty;
    public string? UnitSystem { get; set; } = "imperial";
    public string? WeatherLatLong { get; set; } = "40.7128,74.0060";
    public string? Webhook { get; set; }
    public string? AuthenticationSecret { get; set; }
    public string? AdminPassword { get; set; }

    public void Validate()
    {
        ImmichFrame.Core.Helpers.CalendarSourceValidator.Validate(Calendars);

        // Empty means follow the container's local timezone.
        // Explicit timezone identifiers must be recognized by .NET.
        if (!string.IsNullOrWhiteSpace(TimeZoneId))
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new ArgumentException(
                    "Unknown timezone identifier.",
                    nameof(TimeZoneId));
            }
            catch (InvalidTimeZoneException)
            {
                throw new ArgumentException(
                    "Invalid timezone data.",
                    nameof(TimeZoneId));
            }
        }

        // Limit portrait pairing lookahead to a reasonable range.
        if (SplitViewLookahead < 2 || SplitViewLookahead > 8)
        {
            throw new ArgumentOutOfRangeException(
                nameof(SplitViewLookahead),
                "SplitViewLookahead must be between 2 and 8.");
        }

        // Google Font family names are plain text, never CSS or URLs.
        // Validate only when enabled so existing configurations remain valid.
        if (UseGoogleFont)
        {
            if (string.IsNullOrWhiteSpace(GoogleFontFamily) ||
                GoogleFontFamily.Length > 100 ||
                !System.Text.RegularExpressions.Regex.IsMatch(
                    GoogleFontFamily,
                    @"^[A-Za-z0-9][A-Za-z0-9 &'()\-]*$"))
            {
                throw new ArgumentException(
                    "GoogleFontFamily must be a valid font family name " +
                    "containing only letters, numbers, spaces, " +
                    "apostrophes, parentheses, ampersands, or hyphens.",
                    nameof(GoogleFontFamily));
            }
        }

        if (FrostedGlassOpacity is < 10 or > 95)
        {
            throw new ArgumentOutOfRangeException(
                nameof(FrostedGlassOpacity),
                "FrostedGlassOpacity must be between 10 and 95 percent."
            );
        }

        if (CalendarDaysAhead < 0 || CalendarDaysAhead > 30)
        {
            throw new ArgumentOutOfRangeException(
                nameof(CalendarDaysAhead),
                "CalendarDaysAhead must be between 0 and 30."
            );
        }

        if (CalendarDescriptionMaxCharacters < 40 ||
            CalendarDescriptionMaxCharacters > 500)
        {
            throw new ArgumentOutOfRangeException(
                nameof(CalendarDescriptionMaxCharacters),
                "CalendarDescriptionMaxCharacters must be between 40 and 500."
            );
        }
    }
}

public class ServerAccountSettings : IAccountSettings
{
    public string ImmichServerUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string? ApiKeyFile { get; set; } = null;
    public bool ShowMemories { get; set; } = false;
    public bool ShowFavorites { get; set; } = false;
    public bool ShowArchived { get; set; } = false;
    public bool ShowVideos { get; set; } = false;

    public int? ImagesFromDays { get; set; }
    public DateTime? ImagesFromDate { get; set; }
    public DateTime? ImagesUntilDate { get; set; }
    public List<Guid> Albums { get; set; } = new();
    public List<Guid> ExcludedAlbums { get; set; } = new();
    public List<Guid> People { get; set; } = new();
    public List<Guid> ExcludedPeople { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public int? Rating { get; set; }

    public void ValidateAndInitialize()
    {
        if (!string.IsNullOrWhiteSpace(ApiKeyFile))
        {
            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new Exception("Cannot specify both ApiKey and ApiKeyFile. Please provide only one.");
            }
            ApiKey = File.ReadAllText(ApiKeyFile).Trim();
        }

        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new InvalidOperationException("Either ApiKey or ApiKeyFile must be provided.");
        }
    }
}
