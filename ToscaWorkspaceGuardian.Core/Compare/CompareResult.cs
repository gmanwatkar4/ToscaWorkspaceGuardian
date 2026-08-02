namespace ToscaWorkspaceGuardian.Core.Compare;

public class CompareResult
{
    public List<string> AddedObjects { get; set; } = new();

    public List<string> RemovedObjects { get; set; } = new();

    public List<PropertyChange> PropertyChanges { get; set; } = new();
}