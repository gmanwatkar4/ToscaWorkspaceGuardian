namespace ToscaWorkspaceGuardian.Core.Models;

public class OutputParserResult
{
    public bool Success { get; set; }

    public string RawOutput { get; set; } = string.Empty;

    public WorkspaceSummary WorkspaceSummary { get; set; } = new();
}