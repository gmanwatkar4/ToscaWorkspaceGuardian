using System.Text.Json.Serialization;

namespace ToscaWorkspaceGuardian.Core.AI;

public class OpenRouterRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    [JsonPropertyName("messages")]
    public List<OpenRouterMessage> Messages { get; set; } = new();

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 1000;

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.2;
}

public class OpenRouterMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";
}