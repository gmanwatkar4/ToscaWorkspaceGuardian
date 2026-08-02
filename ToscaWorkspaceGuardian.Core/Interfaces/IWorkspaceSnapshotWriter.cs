using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IWorkspaceSnapshotWriter
{
    Task WriteAsync(
        WorkspaceSnapshot snapshot,
        string filePath,
        CancellationToken cancellationToken = default);
}