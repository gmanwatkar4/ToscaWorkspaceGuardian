// <copyright file="IAIProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

/// <summary>

/// TODO: Describe IAIProvider.

/// </summary>

public interface IAIProvider
{
    Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default);
}

