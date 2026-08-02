// <copyright file="TCShellExecutor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

public class TCShellExecutor
{
    private readonly IToscaInstallationService installationService;
    private readonly IProcessRunner processRunner;
    private readonly OutputWriter outputWriter;

    public TCShellExecutor(
        IToscaInstallationService installationService,
        IProcessRunner processRunner,
        OutputWriter outputWriter)
    {
        this.installationService = installationService;
        this.processRunner = processRunner;
        this.outputWriter = outputWriter;
    }

    public async Task<ExecutionResult> ExecuteAsync(
        string scriptFile,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var installation = this.installationService.GetInstallation();

        if (!installation.IsInstalled)
        {
            return new ExecutionResult
            {
                Success = false,
                ExitCode = -1,
                StandardError = "TCShell installation not found.",
            };
        }

        string workingDirectory = Path.GetDirectoryName(scriptFile)!;

        string outputFile = Path.Combine(
            workingDirectory,
            "Output.txt");

        string errorFile = Path.Combine(
            workingDirectory,
            "Error.txt");

        string arguments =
            $"-workspace \"{request.WorkspacePath}\" " +
            $"-login {request.Username} {request.Password} " +
            $"\"{scriptFile}\"";

        string debugFile = Path.Combine(
    Path.GetTempPath(),
    "ToscaWorkspaceGuardian",
    "CommandLine.txt");

        Directory.CreateDirectory(
            Path.GetDirectoryName(debugFile)!);

        await File.WriteAllTextAsync(
            debugFile,
            installation.TCShellPath + Environment.NewLine + arguments);

        var result = await this.processRunner.ExecuteAsync(
            installation.TCShellPath,
            arguments,
            cancellationToken: cancellationToken);

        await this.outputWriter.WriteAsync(
            outputFile,
            errorFile,
            result.StandardOutput,
            result.StandardError);

        return new ExecutionResult
        {
            Success = result.ExitCode == 0,
            ExitCode = result.ExitCode,
            ScriptPath = scriptFile,
            OutputFile = outputFile,
            ErrorFile = errorFile,
            StandardOutput = result.StandardOutput,
            StandardError = result.StandardError,
        };
    }
}
