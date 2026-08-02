using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IHealthReportExporter
{
    Task ExportAsync(
        IEnumerable<HealthIssue> issues,
        string outputFile,
        CancellationToken cancellationToken = default);
}