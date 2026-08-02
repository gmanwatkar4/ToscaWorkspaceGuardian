using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface ISnapshotBuilder
{
    void AddDocument(
        WorkspaceSnapshot snapshot,
        OutputDocument document);
}