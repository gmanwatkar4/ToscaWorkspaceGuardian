using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.TCShell;

public class TCShellExecutor
{
    private readonly IToscaInstallationService _installationService;
    private readonly IProcessRunner _processRunner;
    private readonly OutputWriter _outputWriter;

    public TCShellExecutor(
        IToscaInstallationService installationService,
        IProcessRunner processRunner,
        OutputWriter outputWriter)
    {
        _installationService = installationService;
        _processRunner = processRunner;
        _outputWriter = outputWriter;
    }

    public async Task<ExecutionResult> ExecuteAsync(
        string scriptFile,
        WorkspaceRequest request)
    {
        var installation = _installationService.GetInstallation();

        if (!installation.IsInstalled)
        {
            return new ExecutionResult
            {
                Success = false,
                ExitCode = -1,
                StandardError = "TCShell installation not found."
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

        var result = await _processRunner.ExecuteAsync(
            installation.TCShellPath,
            arguments);

        await _outputWriter.WriteAsync(
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
            StandardError = result.StandardError
        };
    }
}