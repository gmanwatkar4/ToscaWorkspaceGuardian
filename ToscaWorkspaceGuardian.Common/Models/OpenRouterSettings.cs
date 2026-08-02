namespace ToscaWorkspaceGuardian.Core.AI;

public class OpenRouterSettings
{
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "google/gemini-2.5-flash";

    public int MaxTokens { get; set; } = 1000;

    public double Temperature { get; set; } = 0.2;
}