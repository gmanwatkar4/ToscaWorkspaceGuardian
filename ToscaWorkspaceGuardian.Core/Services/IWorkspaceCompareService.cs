using ToscaWorkspaceGuardian.Core.Compare;

namespace ToscaWorkspaceGuardian.Core.Services;

public interface IWorkspaceCompareService
{
    Task<CompareResult> CompareAsync(
        string oldSnapshotFile,
        string newSnapshotFile,
        CancellationToken cancellationToken = default);
}