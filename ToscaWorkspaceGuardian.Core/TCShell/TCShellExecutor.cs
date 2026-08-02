// <copyright file="TCShellExecutor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using Microsoft.Extensions.Logging;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

public class TCShellExecutor
{
    private readonly IToscaInstallationService installationService;
    private readonly IProcessRunner processRunner;
    private readonly OutputWriter outputWriter;
    private readonly ToscaWorkspaceGuardian.Core.Diagnostics.TelemetryCollector telemetry;
    private readonly Microsoft.Extensions.Logging.ILogger<TCShellExecutor>? logger;

    public TCShellExecutor(
        IToscaInstallationService installationService,
        IProcessRunner processRunner,
        OutputWriter outputWriter,
        ToscaWorkspaceGuardian.Core.Diagnostics.TelemetryCollector telemetry,
        Microsoft.Extensions.Logging.ILogger<TCShellExecutor>? logger = null)
    {
        this.installationService = installationService;
        this.processRunner = processRunner;
        this.outputWriter = outputWriter;
        this.telemetry = telemetry;
        this.logger = logger;
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

        // Write debug command-line file only when explicitly enabled to avoid excessive I/O in normal runs
        if (Environment.GetEnvironmentVariable("TWG_DEBUG_WRITE") == "1")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(debugFile)!);
            await File.WriteAllTextAsync(debugFile, installation.TCShellPath + Environment.NewLine + arguments);
        }
        using var activity = ToscaWorkspaceGuardian.Core.Diagnostics.TelemetrySources.Activity.StartActivity("TCShell.Execute");
        activity?.SetTag("script", scriptFile);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = await this.processRunner.ExecuteAsync(
            installation.TCShellPath,
            arguments,
            cancellationToken: cancellationToken);
        sw.Stop();

        activity?.SetTag("exit_code", result.ExitCode);
        activity?.SetTag("elapsed_ms", sw.Elapsed.TotalMilliseconds);

        try
        {
            this.telemetry?.RecordTCShellCall(sw.Elapsed, result.ExitCode);
            ToscaWorkspaceGuardian.Core.Diagnostics.TelemetrySources.TCShellCalls.Add(1);
        }
        catch { }

        this.logger?.LogDebug("TCShell executed in {Elapsed}ms with exit code {ExitCode}", sw.Elapsed.TotalMilliseconds, result.ExitCode);

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
