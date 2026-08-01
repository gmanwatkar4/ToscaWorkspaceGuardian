using System.Diagnostics;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Models;

namespace ToscaWorkspaceGuardian.Common.Utilities;

public class ProcessRunner : IProcessRunner
{
    public async Task<ProcessResult> ExecuteAsync(
        string fileName,
        string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,

            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,

            CreateNoWindow = true
        };

        using var process = new Process();

        process.StartInfo = startInfo;

        process.Start();

        // Read both streams concurrently to avoid deadlocks
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string output = await outputTask;
        string error = await errorTask;

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = output,
            StandardError = error
        };
    }
}