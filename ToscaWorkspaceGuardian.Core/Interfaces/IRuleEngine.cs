using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IRuleEngine
{
    Task<AnalysisResult> ExecuteAsync(
        WorkspaceSummary workspace,
        CancellationToken cancellationToken = default);
}