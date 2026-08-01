using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Workspace;

public class WorkspaceReader : IWorkspaceReader
{
    private readonly IScriptService _scriptService;
    private readonly ITCShellService _tcShellService;

    public WorkspaceReader(
        IScriptService scriptService,
        ITCShellService tcShellService)
    {
        _scriptService = scriptService;
        _tcShellService = tcShellService;
    }

    public async Task<WorkspaceSummary> ReadAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var script = await _scriptService.GenerateScriptAsync(
            new ScriptRequest
            {
                ScriptType = ScriptType.WorkspaceAnalysis,
                WorkspaceRequest = request,
                Query = "=>SUBPARTS:TestCase"
            },
            cancellationToken);

        var response = await _tcShellService.ExecuteScriptAsync(
            script,
            request,
            cancellationToken);

        return new WorkspaceSummary
        {
            WorkspaceInfo = new WorkspaceInfo
            {
                WorkspacePath = request.WorkspacePath,
                IsManagedRepository = request.IsManagedRepository
            },

            Metadata = new WorkspaceMetadata
            {
                AnalysisTime = DateTime.Now
            },

            Statistics = new WorkspaceStatistics()
        };
    }
}