using System.Text.Json.Serialization;

namespace McpAguiServer.Shared;

internal sealed class Project
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
}
