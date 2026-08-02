// <copyright file="TCShellCommandBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using ToscaWorkspaceGuardian.Core.Models;

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
