// <copyright file="MockAIProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.AI;

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

• Remove unused empty folders.

• Resolve duplicate sibling objects.

• Execute smoke regression after upgrade.
""",
            });
    }
}
