using System.Text.Json.Serialization;

namespace ToscaWorkspaceGuardian.Core.AI;

public class OpenRouterResponse
{
    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public ChoiceMessage? Message { get; set; }
}

public class ChoiceMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}