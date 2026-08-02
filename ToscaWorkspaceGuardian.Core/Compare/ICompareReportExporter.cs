namespace ToscaWorkspaceGuardian.Core.Compare;

public interface ICompareReportExporter
{
    Task ExportAsync(
        CompareResult result,
        string filePath,
        CancellationToken cancellationToken = default);
}