namespace ToscaWorkspaceGuardian.Tests;

using ToscaWorkspaceGuardian.Core.AI;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

public sealed class UpgradeCopilotServiceTests
{
    [Fact]
    public async Task CreateBriefingAsync_PassesVerifiedEvidenceToAiProvider()
    {
        var provider = new CapturingAiProvider();
        var service = new UpgradeCopilotService(provider);
        var analysis = new AnalysisResult
        {
            TotalObjects = 10,
            BlockingIssues = 1,
            HealthIssues = new[]
            {
                new HealthIssue { RuleId = "WG-2026-001", Severity = "High", ObjectName = "Legacy module", Description = "Deprecated attribute" },
            },
        };

        var briefing = await service.CreateBriefingAsync(analysis, "2025.1", "2026.1");

        Assert.Equal("Grounded response", briefing);
        Assert.Contains("WG-2026-001", provider.LastPrompt, StringComparison.Ordinal);
        Assert.Contains("Do not invent", provider.LastPrompt, StringComparison.Ordinal);
    }

    private sealed class CapturingAiProvider : IAIProvider
    {
        public string LastPrompt { get; private set; } = string.Empty;

        public Task<AIResponse> GenerateAsync(AIRequest request, CancellationToken cancellationToken = default)
        {
            this.LastPrompt = request.Prompt;
            return Task.FromResult(new AIResponse { Content = "Grounded response" });
        }
    }
}
