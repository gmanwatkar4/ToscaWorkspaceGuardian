using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Workspace;

public class WorkspaceAnalyzer : IWorkspaceAnalyzer
{
    private readonly IWorkspaceCrawler _crawler;
    private readonly IWorkspaceSnapshotExporter _snapshotExporter;
    private readonly HealthAnalyzer _healthAnalyzer;
    private readonly IHealthReportExporter _healthReportExporter;

    public WorkspaceAnalyzer(
     IWorkspaceCrawler crawler,
     IWorkspaceSnapshotExporter snapshotExporter,
     HealthAnalyzer healthAnalyzer,
     IHealthReportExporter healthReportExporter)
    {
        _crawler = crawler;
        _snapshotExporter = snapshotExporter;
        _healthAnalyzer = healthAnalyzer;
        _healthReportExporter = healthReportExporter;
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

        //------------------------------------------
        // Return Result
        //------------------------------------------

        return new AnalysisResult
        {
            Success = true,
            Message =
                $"Workspace analyzed successfully.{Environment.NewLine}" +
                $"Objects : {snapshot.Objects.Count}{Environment.NewLine}" +
                $"Health Issues : {issues.Count}"
        };
    }
}