namespace ToscaWorkspaceGuardian.Core.Compare;

public class PropertyChange
{
    public string NodePath { get; set; } = string.Empty;

    public string Property { get; set; } = string.Empty;

    public string OldValue { get; set; } = string.Empty;

    public string NewValue { get; set; } = string.Empty;
}