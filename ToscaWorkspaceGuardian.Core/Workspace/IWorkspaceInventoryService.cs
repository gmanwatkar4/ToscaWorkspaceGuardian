namespace ToscaWorkspaceGuardian.Core.Workspace;

using ToscaWorkspaceGuardian.Core.Models;

/// <summary>Creates a complete, read-only JSON inventory of a Tosca workspace.</summary>
public interface IWorkspaceInventoryService
{
    Task<WorkspaceInventoryResult> AnalyzeAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
