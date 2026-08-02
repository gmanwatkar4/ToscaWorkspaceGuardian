// <copyright file="ScriptService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script;

using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

public class ScriptService : IScriptService
{
    private readonly ScriptComposer composer;
    private readonly RepositoryScanScriptBuilder repositoryBuilder;
    private readonly PrintObjectScriptBuilder printBuilder;

    public ScriptService(
        ScriptComposer composer,
        RepositoryScanScriptBuilder repositoryBuilder,
        PrintObjectScriptBuilder printBuilder)
    {
        this.composer = composer;
        this.repositoryBuilder = repositoryBuilder;
        this.printBuilder = printBuilder;
    }

    public async Task<string> GenerateScriptAsync(
        ScriptRequest request,
        CancellationToken cancellationToken = default)
    {
        string directory =
            Path.Combine(
                Path.GetTempPath(),
                "ToscaWorkspaceGuardian");

        Directory.CreateDirectory(directory);

        if (request.ScriptType == ScriptType.RepositoryScan)
        {
            string repositoryScript =
                this.repositoryBuilder.Build(request.Queries);

            string repositoryFile =
                Path.Combine(directory, "RepositoryScan.tcs");

            await File.WriteAllTextAsync(
                repositoryFile,
                repositoryScript,
                cancellationToken);

            string printFile =
                Path.Combine(directory, "PrintObject.tcs");

            await File.WriteAllTextAsync(
                printFile,
                this.printBuilder.Build(),
                cancellationToken);

            return repositoryFile;
        }

        string script =
            this.composer.Compose(request);

        string workspaceFile =
            Path.Combine(directory, "WorkspaceAnalysis.tcs");

        await File.WriteAllTextAsync(
            workspaceFile,
            script,
            cancellationToken);

        return workspaceFile;
    }
}
