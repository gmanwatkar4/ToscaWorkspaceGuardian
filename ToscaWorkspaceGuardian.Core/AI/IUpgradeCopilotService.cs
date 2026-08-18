namespace ToscaWorkspaceGuardian.Core.AI;

using ToscaWorkspaceGuardian.Core.Models;

public interface IUpgradeCopilotService
{
    Task<string> CreateBriefingAsync(
        AnalysisResult analysis,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default);

    Task<string> AnswerAsync(
        AnalysisResult analysis,
        string sourceVersion,
        string targetVersion,
        string question,
        CancellationToken cancellationToken = default);
}
