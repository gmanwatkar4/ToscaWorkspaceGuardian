// <copyright file="OutputDocumentExporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Models;

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
                WriteIndented = true,
            });

        await File.WriteAllTextAsync(file, json);
    }
}
