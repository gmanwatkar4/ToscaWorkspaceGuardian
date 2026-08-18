namespace ToscaWorkspaceGuardian.Core.Repository;

using ToscaWorkspaceGuardian.Core.Subset;

/// <summary>
/// Read-only inventory of a local Tosca repository database.
/// </summary>
public sealed class RepositoryDatabaseAnalysisResult
{
    public string DatabasePath { get; init; } = string.Empty;

    public int TotalObjects { get; init; }

    public int ObjectTypes { get; init; }

    public IReadOnlyList<RepositoryTypeCount> TypeCounts { get; init; } = Array.Empty<RepositoryTypeCount>();

    public IReadOnlyList<SubsetFinding> Findings { get; init; } = Array.Empty<SubsetFinding>();
}
