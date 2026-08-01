using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.TCShell;

public class OutputDocumentExporter
{
    public async Task ExportAsync(
        OutputDocument document,
        string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);

        string file = Path.Combine(
            outputFolder,
            "ParsedOutput.json");

        string json = JsonSerializer.Serialize(
            document,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        await File.WriteAllTextAsync(file, json);
    }
}