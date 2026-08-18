namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>
/// Produces support-ready upgrade guidance from verified Workspace Guardian findings.
/// </summary>
public sealed class UpgradeCopilotService : IUpgradeCopilotService
{
    private readonly IAIProvider aiProvider;

    public UpgradeCopilotService(IAIProvider aiProvider)
    {
        this.aiProvider = aiProvider;
    }

    public Task<string> CreateBriefingAsync(
        AnalysisResult analysis,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default)
    {
        return this.GenerateAsync(analysis, sourceVersion, targetVersion, "Create the upgrade support briefing.", cancellationToken);
    }

    public Task<string> AnswerAsync(
        AnalysisResult analysis,
        string sourceVersion,
        string targetVersion,
        string question,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(question);
        return this.GenerateAsync(analysis, sourceVersion, targetVersion, question, cancellationToken);
    }

    private async Task<string> GenerateAsync(
        AnalysisResult analysis,
        string sourceVersion,
        string targetVersion,
        string request,
        CancellationToken cancellationToken)
    {
        var response = await this.aiProvider.GenerateAsync(
            new AIRequest
            {
                Prompt = BuildPrompt(analysis, sourceVersion, targetVersion, request),
            },
            cancellationToken);

        return string.IsNullOrWhiteSpace(response.Content)
            ? "The AI provider returned no guidance. Review the verified findings below."
            : response.Content.Trim();
    }

    private static string BuildPrompt(AnalysisResult analysis, string sourceVersion, string targetVersion, string request)
    {
        var evidence = new StringBuilder();
        foreach (var issue in analysis.HealthIssues.Take(20))
        {
            evidence.AppendLine($"- [{issue.Severity}] {issue.RuleId}: {issue.ObjectName}; path: {issue.NodePath}; {issue.Description}");
        }

        foreach (var recommendation in analysis.UpgradeRecommendations.Take(12))
        {
            evidence.AppendLine($"- Upgrade rule: {recommendation}");
        }

        return $"""
            You are the Tosca Upgrade Copilot for an internal support team.
            Upgrade path: Tosca {sourceVersion} to {targetVersion}.

            Use only the verified evidence below. Do not invent modules, test cases, Tosca behavior, release-note changes, counts, or documentation references. If evidence is insufficient, say what TQL or workspace scan is needed.

            Verified scan totals:
            - Total objects: {analysis.TotalObjects}
            - Health score: {analysis.HealthScore}
            - Health issues: {analysis.HealthIssueCount}
            - Blockers: {analysis.BlockingIssues}
            - Warnings: {analysis.Warnings}

            Verified evidence:
            {evidence}

            User request: {request}

            Respond concisely with these headings:
            Risk summary
            Impacted evidence
            Recommended support action
            Next validation
            """;
    }
}
