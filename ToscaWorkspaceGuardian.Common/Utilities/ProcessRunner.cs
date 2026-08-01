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

            RedirectStandardOutput = true,
            RedirectStandardError = true,

            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();

        process.StartInfo = startInfo;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();

        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = output,
            StandardError = error
        };
    }
}