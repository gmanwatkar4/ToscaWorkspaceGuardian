namespace ToscaWorkspaceGuardian.Core.Models;

public sealed class AnalysisProgress
{
    public int Percentage { get; init; }

    public string Message { get; init; } = string.Empty;
}
