namespace ToscaWorkspaceGuardian.Core.Subset;

/// <summary>
/// Inventory and upgrade findings produced from a Tosca subset file.
/// </summary>
public sealed class SubsetAnalysisResult
{
    public string ProjectName { get; init; } = string.Empty;

    public int EntityCount { get; init; }

    public IReadOnlyDictionary<string, int> EntityCounts { get; init; } = new Dictionary<string, int>();

    public IReadOnlyList<SubsetFinding> Findings { get; init; } = Array.Empty<SubsetFinding>();
}
