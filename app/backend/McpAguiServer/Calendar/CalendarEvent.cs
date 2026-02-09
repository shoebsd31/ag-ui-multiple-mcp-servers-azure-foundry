using System.Text.Json.Serialization;

namespace McpAguiServer.Calendar;

internal sealed class CalendarEvent
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime End { get; set; }

    [JsonPropertyName("type")]
    public EventType Type { get; set; }

    [JsonPropertyName("projectCode")]
    public string? ProjectCode { get; set; }

    [JsonPropertyName("projectName")]
    public string? ProjectName { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("attendees")]
    public List<string>? Attendees { get; set; }

    [JsonPropertyName("isRecurring")]
    public bool IsRecurring { get; set; }
}
