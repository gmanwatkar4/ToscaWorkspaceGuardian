// <copyright file="JsonCompareReportExporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Compare;

using System.Text.Json;

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
            WriteIndented = true,
        };

        await File.WriteAllTextAsync(
            filePath,
            JsonSerializer.Serialize(result, options),
            cancellationToken);
    }
}
