using System.Text.Json.Serialization;

namespace McpAguiServer.KnowledgeBase;

internal sealed class KnowledgeArticle
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("projectCode")]
    public string ProjectCode { get; set; } = "";

    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; } = "";

    [JsonPropertyName("viewCount")]
    public int ViewCount { get; set; }
}
