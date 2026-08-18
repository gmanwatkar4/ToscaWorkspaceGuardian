// <copyright file="WorkspaceAnalyzer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Workspace;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Reporting;
using ToscaWorkspaceGuardian.Core.Upgrade;

/// <summary>

/// TODO: Describe WorkspaceAnalyzer.

/// </summary>

public class WorkspaceAnalyzer : IWorkspaceAnalyzer
{
    private readonly IWorkspaceCrawler crawler;
    private readonly IWorkspaceSnapshotExporter snapshotExporter;
    private readonly HealthAnalyzer healthAnalyzer;
    private readonly IHealthReportExporter healthReportExporter;
    private readonly RepositoryStatisticsBuilder statisticsBuilder;
    private readonly WorkspaceHtmlReportGenerator htmlGenerator;
    private readonly UpgradeReadinessAnalyzer upgradeAnalyzer;

    public WorkspaceAnalyzer(
     IWorkspaceCrawler crawler,
     IWorkspaceSnapshotExporter snapshotExporter,
     HealthAnalyzer healthAnalyzer,
     IHealthReportExporter healthReportExporter,
     RepositoryStatisticsBuilder statisticsBuilder,
     WorkspaceHtmlReportGenerator htmlGenerator,
     UpgradeReadinessAnalyzer upgradeAnalyzer)
    {
        this.crawler = crawler;
        this.snapshotExporter = snapshotExporter;
        this.healthAnalyzer = healthAnalyzer;
        this.healthReportExporter = healthReportExporter;
        this.statisticsBuilder = statisticsBuilder;
        this.htmlGenerator = htmlGenerator;
        this.upgradeAnalyzer = upgradeAnalyzer;
    }

    public async Task<AnalysisResult> AnalyzeAsync(
        WorkspaceRequest request,
        IProgress<AnalysisProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        progress?.Report(new AnalysisProgress { Percentage = 5, Message = "Scanning the Tosca workspace..." });
        cancellationToken.ThrowIfCancellationRequested();

        //------------------------------------------
        // Crawl Workspace
        //------------------------------------------
        WorkspaceSnapshot snapshot =
            await this.crawler.CrawlAsync(
                request,
                cancellationToken: cancellationToken);

        progress?.Report(new AnalysisProgress { Percentage = 40, Message = "Writing workspace snapshot..." });
        cancellationToken.ThrowIfCancellationRequested();

        string outputFolder = string.IsNullOrWhiteSpace(request.OutputFolder)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ToscaWorkspaceGuardian")
            : request.OutputFolder;

        string snapshotFile = Path.Combine(
            outputFolder,
            "Snapshot.json");

        await this.snapshotExporter.ExportAsync(
            snapshot,
            snapshotFile,
            cancellationToken);

        progress?.Report(new AnalysisProgress { Percentage = 55, Message = "Running health rules..." });
        cancellationToken.ThrowIfCancellationRequested();

        var issues = this.healthAnalyzer.Analyze(snapshot);

        string healthReportFile = Path.Combine(
            outputFolder,
            "HealthReport.json");

        await this.healthReportExporter.ExportAsync(
            issues,
            healthReportFile,
            cancellationToken);

        System.Diagnostics.Debug.WriteLine(
            $"Health Issues : {issues.Count}");

        var statistics =
        this.statisticsBuilder.Build(snapshot, issues);

        progress?.Report(new AnalysisProgress { Percentage = 70, Message = "Generating HTML report..." });
        cancellationToken.ThrowIfCancellationRequested();

        System.Diagnostics.Debug.WriteLine(
            $"Health Score : {statistics.HealthScore}");

        System.Diagnostics.Debug.WriteLine(
            $"Modules : {statistics.ModuleCount}");

        string reportFile = Path.Combine(
            outputFolder,
            "WorkspaceReport.html");

        await this.htmlGenerator.GenerateAsync(
            statistics,
            issues,
            reportFile);

        progress?.Report(new AnalysisProgress { Percentage = 85, Message = "Checking upgrade readiness..." });
        cancellationToken.ThrowIfCancellationRequested();

        var upgradeReport =
    this.upgradeAnalyzer.Analyze(
        statistics,
        issues,
        request.SourceVersion,
        request.TargetVersion);

        progress?.Report(new AnalysisProgress { Percentage = 100, Message = "Analysis complete." });

        //------------------------------------------
        // Return Result
        //------------------------------------------
        return new AnalysisResult
        {
            Success = true,
            TotalObjects = statistics.TotalObjects,
            HealthScore = statistics.HealthScore,
            HealthIssueCount = statistics.HealthIssueCount,
            BlockingIssues = upgradeReport.BlockingIssues,
            Warnings = upgradeReport.Warnings,
            ReadyForUpgrade = upgradeReport.ReadyForUpgrade,
            SnapshotFile = snapshotFile,
            HealthReportFile = healthReportFile,
            HtmlReportFile = reportFile,
            HealthIssues = issues,
            UpgradeRecommendations = upgradeReport.Recommendations
                .Concat(upgradeReport.CompatibilityRules.Select(rule => $"{rule.Severity}: {rule.Recommendation}"))
                .ToList(),
            Message =

            $"""

Upgrade Readiness

Status :
{(upgradeReport.ReadyForUpgrade ? "READY" : "ATTENTION")}

Blocking Issues :
{upgradeReport.BlockingIssues}

Warnings :
{upgradeReport.Warnings}
""",
        };
    }
}

