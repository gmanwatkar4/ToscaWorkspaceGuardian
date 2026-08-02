// <copyright file="TCShellService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe TCShellService.

/// </summary>

public class TCShellService : ITCShellService
{
    private readonly TCShellExecutor executor;

    public TCShellService(
        TCShellExecutor executor)
    {
        this.executor = executor;
    }

    public async Task<TCShellResponse> ExecuteScriptAsync(
        string scriptFile,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var execution =
            await this.executor.ExecuteAsync(
                scriptFile,
                request,
                cancellationToken: cancellationToken);

        return new TCShellResponse
        {
            Success = execution.Success,
            ExitCode = execution.ExitCode,
            Output = execution.StandardOutput,
            Error = execution.StandardError,
        };
    }
}

