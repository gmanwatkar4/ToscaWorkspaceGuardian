namespace ToscaWorkspaceGuardian.Core.Models;

public class ScriptRequest
{
    public ScriptType ScriptType { get; set; }

    public WorkspaceRequest WorkspaceRequest { get; set; } = new();

    public string Query { get; set; } = string.Empty;

    public string OutputDirectory { get; set; } = string.Empty;
}