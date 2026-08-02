// <copyright file="OutputWriter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

public class OutputWriter
{
    public async Task WriteAsync(
        string outputFile,
        string errorFile,
        string output,
        string error)
    {
        var directory = Path.GetDirectoryName(outputFile)!;

        Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(outputFile, output);

        await File.WriteAllTextAsync(errorFile, error);
    }
}
