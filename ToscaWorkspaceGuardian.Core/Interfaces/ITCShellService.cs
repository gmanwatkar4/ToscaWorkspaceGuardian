// <copyright file="ITCShellService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe ITCShellService.

/// </summary>

public interface ITCShellService
{
    Task<TCShellResponse> ExecuteScriptAsync(
        string scriptFile,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}

