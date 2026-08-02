namespace ToscaWorkspaceGuardian.Core.Models;

public class OutputObject
{
    public bool IsContainer =>
    ObjectType == "TCProject" ||
    ObjectType == "TCFolder" ||
    ObjectType == "OwnedFolder" ||
    ObjectType == "ExecutionEntryFolder";

    public string ObjectType { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string RawText { get; set; } = string.Empty;

    public List<OutputProperty> Properties { get; set; } = new();

    public Dictionary<string, List<string>> Collections { get; set; } = new();

    //--------------------------------------------------------
    // Helper Methods
    //--------------------------------------------------------

    public string? GetProperty(string propertyName)
    {
        return Properties
            .FirstOrDefault(x => x.Name == propertyName)
            ?.Value;
    }

    public IReadOnlyList<string> GetCollection(string collectionName)
    {
        if (Collections.TryGetValue(collectionName, out var values))
            return values;

        return Array.Empty<string>();
    }

    public bool HasCollection(string collectionName)
    {
        return Collections.ContainsKey(collectionName);
    }
}