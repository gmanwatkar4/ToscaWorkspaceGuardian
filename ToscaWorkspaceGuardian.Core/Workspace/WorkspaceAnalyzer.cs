using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Workspace;

public class WorkspaceAnalyzer : IWorkspaceAnalyzer
{
    private readonly IWorkspaceReader _workspaceReader;

    public WorkspaceAnalyzer(
        IWorkspaceReader workspaceReader)
    {
        _workspaceReader = workspaceReader;
    }

    public async Task<AnalysisResult> AnalyzeAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var summary =
            await _workspaceReader.ReadAsync(
                request,
                cancellationToken);

        return new AnalysisResult
        {
            Success = true,
            Message = "Workspace analysis completed.",
            Summary = summary
        };
    }
}