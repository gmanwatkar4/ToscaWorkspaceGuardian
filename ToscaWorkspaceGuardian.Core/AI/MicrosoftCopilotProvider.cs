// <copyright file="MicrosoftCopilotProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

using System.Threading;

/// <summary>

/// TODO: Describe MicrosoftCopilotProvider.

/// </summary>

public class MicrosoftCopilotProvider : IAIProvider
{
    public Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

