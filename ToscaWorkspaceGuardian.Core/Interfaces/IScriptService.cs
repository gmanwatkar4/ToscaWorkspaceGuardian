// <copyright file="IScriptService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe IScriptService.

/// </summary>

public interface IScriptService
{
    Task<string> GenerateScriptAsync(
        ScriptRequest request,
        CancellationToken cancellationToken = default);
}

