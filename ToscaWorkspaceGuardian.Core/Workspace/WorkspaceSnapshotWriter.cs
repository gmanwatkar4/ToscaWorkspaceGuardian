using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

namespace ToscaWorkspaceGuardian.Core.Workspace;

public class WorkspaceSnapshotWriter : IWorkspaceSnapshotWriter
{
    public async Task WriteAsync(
        WorkspaceSnapshot snapshot,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json =
            JsonSerializer.Serialize(snapshot, options);

        await File.WriteAllTextAsync(
            filePath,
            json,
            cancellationToken);
    }
}