using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IWorkspaceSnapshotExporter
{
    Task ExportAsync(
        WorkspaceSnapshot snapshot,
        string outputFile,
        CancellationToken cancellationToken = default);
}