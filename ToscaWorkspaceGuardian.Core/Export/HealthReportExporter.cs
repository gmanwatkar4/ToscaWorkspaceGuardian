using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

namespace ToscaWorkspaceGuardian.Core.Export;

public class HealthReportExporter : IHealthReportExporter
{
    public async Task ExportAsync(
        IEnumerable<HealthIssue> issues,
        string outputFile,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(
            issues,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        Directory.CreateDirectory(
            Path.GetDirectoryName(outputFile)!);

        await File.WriteAllTextAsync(
            outputFile,
            json,
            cancellationToken);
    }
}