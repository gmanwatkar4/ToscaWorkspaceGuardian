// <copyright file="MockAIProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

/// <summary>

/// TODO: Describe MockAIProvider.

/// </summary>

public class MockAIProvider : IAIProvider
{
    public Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            new AIResponse
            {
                Content =
"""
Repository Health Summary

Health Score: Good

Recommendations

â€¢ Remove unused empty folders.

â€¢ Resolve duplicate sibling objects.

â€¢ Execute smoke regression after upgrade.
""",
            });
    }
}

