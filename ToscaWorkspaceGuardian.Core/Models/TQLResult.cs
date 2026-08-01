namespace ToscaWorkspaceGuardian.Core.Models;

public class TQLResult
{
    public bool Success { get; set; }

    public string Query { get; set; } = string.Empty;

    public string Output { get; set; } = string.Empty;

    public string Error { get; set; } = string.Empty;
}