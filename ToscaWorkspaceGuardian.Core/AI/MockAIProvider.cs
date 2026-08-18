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
        if (request.Prompt.Contains("translate a Tosca workspace question", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new AIResponse { Content = "=>SUBPARTS:TestCase" });
        }

        if (request.Prompt.Contains("Returned object count", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new AIResponse { Content = "Result: The read-only workspace query completed.\nEvidence: Review the returned Tosca objects below.\nNext step: Refine the question to focus on a specific module or attribute." });
        }

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

