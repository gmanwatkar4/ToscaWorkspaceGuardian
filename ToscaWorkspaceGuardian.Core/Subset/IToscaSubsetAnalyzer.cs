namespace ToscaWorkspaceGuardian.Core.Subset;

public interface IToscaSubsetAnalyzer
{
    Task<SubsetAnalysisResult> AnalyzeAsync(
        string subsetPath,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default);
}
