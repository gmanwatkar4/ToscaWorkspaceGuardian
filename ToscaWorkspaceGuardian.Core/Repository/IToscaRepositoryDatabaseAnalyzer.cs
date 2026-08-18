namespace ToscaWorkspaceGuardian.Core.Repository;

public interface IToscaRepositoryDatabaseAnalyzer
{
    Task<RepositoryDatabaseAnalysisResult> AnalyzeAsync(
        string databasePath,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default);
}
