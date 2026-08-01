namespace ToscaWorkspaceGuardian.Core.Models;

public class OutputObject
{
    public string ObjectType { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string RawText { get; set; } = string.Empty;

    public List<OutputProperty> Properties { get; set; } = new();

    public Dictionary<string, List<string>> Collections { get; set; } = new();
}