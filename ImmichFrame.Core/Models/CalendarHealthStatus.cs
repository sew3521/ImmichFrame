namespace ImmichFrame.Core.Models;

public class CalendarHealthStatus
{
    public Guid CalendarId { get; init; }

    public string CalendarName { get; init; } = string.Empty;

    public string CalendarColor { get; init; } = "#6299EC";

    public bool Available { get; init; }

    public bool UsingFallback { get; init; }

    public bool ShowWarning => !Available && !UsingFallback;
}
