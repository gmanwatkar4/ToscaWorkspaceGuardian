using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IWorkspaceDetector
{
    WorkspaceInfo Detect(string workspacePath);
}