using System.Text.Json.Serialization;

namespace McpAguiServer.Calendar;

[JsonConverter(typeof(JsonStringEnumConverter<EventType>))]
public enum EventType
{
    Meeting,
    FocusBlock,
    Deadline
}
