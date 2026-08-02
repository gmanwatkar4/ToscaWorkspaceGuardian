// <copyright file="IAIProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

public interface IAIProvider
{
    Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default);
}
