using System.Collections.Generic;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Workspace;

public class RepositoryTreeWalker : IRepositoryTreeWalker
{
    public async Task<ParsedWorkspace> ScanAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var queue = new Queue<RepositoryNode>();

        queue.Enqueue(
            new RepositoryNode
            {
                NodePath = "/"
            });

        while (queue.Count > 0)
        {
            RepositoryNode current =
                queue.Dequeue();

            // Sprint 6.3D
            // JumpToNode(current.NodePath)
            // Print
            // Parse
            // Enqueue Children
        }

        await Task.CompletedTask;

        return new ParsedWorkspace();
    }
}