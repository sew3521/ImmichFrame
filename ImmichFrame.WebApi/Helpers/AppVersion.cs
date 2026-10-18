using System.Reflection;

namespace ImmichFrame.WebApi.Helpers;

public static class AppVersion
{
    public static string Current =>
        Resolve(
            Environment.GetEnvironmentVariable("APP_VERSION"),
            typeof(AppVersion).Assembly.GetName().Version?.ToString()
        );

    public static string Resolve(string? configured, string? assemblyVersion)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured.Trim();
        }

        return string.IsNullOrWhiteSpace(assemblyVersion)
            ? "unknown"
            : assemblyVersion;
    }
}
