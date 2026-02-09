using System.Text.Json.Serialization;

namespace McpAguiServer.SecurityIssues;

[JsonConverter(typeof(JsonStringEnumConverter<IssueStatus>))]
public enum IssueStatus
{
    Open,
    InProgress,
    Resolved,
    Dismissed
}
