using System.Text.Json.Serialization;

namespace McpAguiServer.SecurityIssues;

internal sealed class SecurityIssue
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("projectCode")]
    public string ProjectCode { get; set; } = "";

    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("severity")]
    public Severity Severity { get; set; }

    [JsonPropertyName("status")]
    public IssueStatus Status { get; set; }

    [JsonPropertyName("reportedDate")]
    public DateTime ReportedDate { get; set; }

    [JsonPropertyName("resolvedDate")]
    public DateTime? ResolvedDate { get; set; }

    [JsonPropertyName("reportedBy")]
    public string ReportedBy { get; set; } = "";

    [JsonPropertyName("assignedTo")]
    public string? AssignedTo { get; set; }

    [JsonPropertyName("affectedComponent")]
    public string AffectedComponent { get; set; } = "";

    [JsonPropertyName("recommendation")]
    public string? Recommendation { get; set; }
}
