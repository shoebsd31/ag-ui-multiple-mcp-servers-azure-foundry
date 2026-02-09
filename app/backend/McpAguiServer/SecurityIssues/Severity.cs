using System.Text.Json.Serialization;

namespace McpAguiServer.SecurityIssues;

[JsonConverter(typeof(JsonStringEnumConverter<Severity>))]
public enum Severity
{
    Critical,
    High,
    Medium,
    Low
}
