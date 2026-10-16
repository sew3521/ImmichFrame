namespace ImmichFrame.Core.Models;

public class CalendarSource
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "Calendar";

    public string Url { get; set; } = string.Empty;

    public string Color { get; set; } = "#6299EC";

    public bool Enabled { get; set; } = true;
}
