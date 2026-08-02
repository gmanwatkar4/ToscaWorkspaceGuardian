using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Compare;

namespace ToscaWorkspaceGuardian.Core.Services;

public class WorkspaceCompareService : IWorkspaceCompareService
{
    private readonly WorkspaceSnapshotComparer _comparer;

    public WorkspaceCompareService(
        WorkspaceSnapshotComparer comparer)
    {
        _comparer = comparer;
    }

    public async Task<CompareResult> CompareAsync(
        string oldSnapshotFile,
        string newSnapshotFile,
        CancellationToken cancellationToken = default)
    {
        var oldJson = await File.ReadAllTextAsync(
            oldSnapshotFile,
            cancellationToken);

        var newJson = await File.ReadAllTextAsync(
            newSnapshotFile,
            cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var oldSnapshot =
            JsonSerializer.Deserialize<WorkspaceSnapshot>(
                oldJson,
                options)
            ?? new WorkspaceSnapshot();

        var newSnapshot =
            JsonSerializer.Deserialize<WorkspaceSnapshot>(
                newJson,
                options)
            ?? new WorkspaceSnapshot();

        return _comparer.Compare(
            oldSnapshot,
            newSnapshot);
    }
}