namespace ToscaWorkspaceGuardian.Core.Business;

public class RepositoryObject
{
    public string NodePath { get; set; } = string.Empty;

    public string ObjectType { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public Dictionary<string, string> Properties { get; }
        = new();

    public Dictionary<string, List<string>> Collections { get; }
        = new();
}