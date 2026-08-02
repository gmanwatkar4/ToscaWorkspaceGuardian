// <copyright file="ITQLService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Models;

public interface ITQLService
{
    Task<TQLResult> ExecuteAsync(
        string query,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
