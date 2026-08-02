using System.Text.Json;

namespace ToscaWorkspaceGuardian.Core.Compare;

public class JsonCompareReportExporter : ICompareReportExporter
{
    public async Task ExportAsync(
        CompareResult result,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(
            Path.GetDirectoryName(filePath)!);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        await File.WriteAllTextAsync(
            filePath,
            JsonSerializer.Serialize(result, options),
            cancellationToken);
    }
}