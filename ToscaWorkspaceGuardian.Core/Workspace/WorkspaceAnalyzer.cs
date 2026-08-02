// <copyright file="WorkspaceAnalyzer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Workspace;

using ToscaWorkspaceGuardian.Core.AI;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Reporting;
using ToscaWorkspaceGuardian.Core.Upgrade;

public class WorkspaceAnalyzer : IWorkspaceAnalyzer
{
    private readonly IWorkspaceCrawler crawler;
    private readonly IWorkspaceSnapshotExporter snapshotExporter;
    private readonly HealthAnalyzer healthAnalyzer;
    private readonly IHealthReportExporter healthReportExporter;
    private readonly RepositoryStatisticsBuilder statisticsBuilder;
    private readonly WorkspaceHtmlReportGenerator htmlGenerator;
    private readonly UpgradeReadinessAnalyzer upgradeAnalyzer;
    private readonly IAIProvider aiProvider;

    public WorkspaceAnalyzer(
     IWorkspaceCrawler crawler,
     IWorkspaceSnapshotExporter snapshotExporter,
     HealthAnalyzer healthAnalyzer,
     IHealthReportExporter healthReportExporter,
     RepositoryStatisticsBuilder statisticsBuilder,
     WorkspaceHtmlReportGenerator htmlGenerator,
     UpgradeReadinessAnalyzer upgradeAnalyzer,
     IAIProvider aiProvider)
    {
        this.crawler = crawler;
        this.snapshotExporter = snapshotExporter;
        this.healthAnalyzer = healthAnalyzer;
        this.healthReportExporter = healthReportExporter;
        this.statisticsBuilder = statisticsBuilder;
        this.htmlGenerator = htmlGenerator;
        this.upgradeAnalyzer = upgradeAnalyzer;
        this.aiProvider = aiProvider;
    }

    public async Task<AnalysisResult> AnalyzeAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        //------------------------------------------
        // Crawl Workspace
        //------------------------------------------
        WorkspaceSnapshot snapshot =
            await this.crawler.CrawlAsync(
                request,
                cancellationToken);

        string outputFolder = Path.Combine(
        Environment.GetFolderPath(
        Environment.SpecialFolder.MyDocuments),
        "ToscaWorkspaceGuardian");

        string snapshotFile = Path.Combine(
            outputFolder,
            "Snapshot.json");

        await this.snapshotExporter.ExportAsync(
            snapshot,
            snapshotFile,
            cancellationToken);

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

        var upgradeReport =
    this.upgradeAnalyzer.Analyze(
        statistics,
        issues,
        request.SourceVersion,
        request.TargetVersion);

        var aiResponse = await this.aiProvider.GenerateAsync(
    new AIRequest
    {
        Prompt = """
You are an expert Tosca repository analyzer.

Reply with exactly:

Workspace Guardian AI integration successful.
""",
    },
    cancellationToken);

        System.Diagnostics.Debug.WriteLine(aiResponse.Content);

        //------------------------------------------
        // Return Result
        //------------------------------------------
        return new AnalysisResult
        {
            Success = true,
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
