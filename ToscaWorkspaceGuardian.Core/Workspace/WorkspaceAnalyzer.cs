using ToscaWorkspaceGuardian.Core.AI;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Reporting;
using ToscaWorkspaceGuardian.Core.Upgrade;

namespace ToscaWorkspaceGuardian.Core.Workspace;

public class WorkspaceAnalyzer : IWorkspaceAnalyzer
{
    private readonly IWorkspaceCrawler _crawler;
    private readonly IWorkspaceSnapshotExporter _snapshotExporter;
    private readonly HealthAnalyzer _healthAnalyzer;
    private readonly IHealthReportExporter _healthReportExporter;
    private readonly RepositoryStatisticsBuilder _statisticsBuilder;
    private readonly WorkspaceHtmlReportGenerator _htmlGenerator;
    private readonly UpgradeReadinessAnalyzer _upgradeAnalyzer;
    private readonly IAIProvider _aiProvider;


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
        _crawler = crawler;
        _snapshotExporter = snapshotExporter;
        _healthAnalyzer = healthAnalyzer;
        _healthReportExporter = healthReportExporter;
        _statisticsBuilder = statisticsBuilder;
        _htmlGenerator = htmlGenerator;
        _upgradeAnalyzer = upgradeAnalyzer;
        _aiProvider = aiProvider;
    }

    public async Task<AnalysisResult> AnalyzeAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        //------------------------------------------
        // Crawl Workspace
        //------------------------------------------

        WorkspaceSnapshot snapshot =
            await _crawler.CrawlAsync(
                request,
                cancellationToken);

        string outputFolder = Path.Combine(
        Environment.GetFolderPath(
        Environment.SpecialFolder.MyDocuments),
        "ToscaWorkspaceGuardian");

        string snapshotFile = Path.Combine(
            outputFolder,
            "Snapshot.json");

        await _snapshotExporter.ExportAsync(
            snapshot,
            snapshotFile,
            cancellationToken);

        var issues = _healthAnalyzer.Analyze(snapshot);

        string healthReportFile = Path.Combine(outputFolder,
            "HealthReport.json");

        await _healthReportExporter.ExportAsync(
            issues,
            healthReportFile,
            cancellationToken);

        System.Diagnostics.Debug.WriteLine(
            $"Health Issues : {issues.Count}");

        var statistics =
        _statisticsBuilder.Build(snapshot, issues);

        System.Diagnostics.Debug.WriteLine(
            $"Health Score : {statistics.HealthScore}");

        System.Diagnostics.Debug.WriteLine(
            $"Modules : {statistics.ModuleCount}");


        string reportFile = Path.Combine(
            outputFolder,
            "WorkspaceReport.html");

        await _htmlGenerator.GenerateAsync(
            statistics,
            issues,
            reportFile);

        var upgradeReport =
    _upgradeAnalyzer.Analyze(
        statistics,
        issues,
        request.SourceVersion,
        request.TargetVersion);

        var aiResponse = await _aiProvider.GenerateAsync(
    new AIRequest
    {
        Prompt = """
You are an expert Tosca repository analyzer.

Reply with exactly:

Workspace Guardian AI integration successful.
"""
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
"""

        };


    }
}