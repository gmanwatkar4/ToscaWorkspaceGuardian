using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.TCShell;

public class TCShellCommandBuilder
{
    public string BuildWorkspaceOpenCommand(
        WorkspaceRequest request)
    {
        if (request.IsManagedRepository)
        {
            return
                $"-workspace \"{request.WorkspacePath}\" " +
                $"-auth {request.ClientId}:{request.ClientSecret}";
        }

        return
            $"-workspace \"{request.WorkspacePath}\" " +
            $"-login {request.Username} {request.Password}";
    }
}