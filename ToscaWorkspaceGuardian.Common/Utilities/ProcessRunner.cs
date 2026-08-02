using System.Diagnostics;
using System.Text;
using System.Threading;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Models;

namespace ToscaWorkspaceGuardian.Common.Utilities;

public class ProcessRunner : IProcessRunner
{
    public async Task<ProcessResult> ExecuteAsync(
        string fileName,
        string arguments,
        CancellationToken cancellationToken = default)
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

        try
        {
            process.Start();

            // Read both streams concurrently to avoid deadlocks
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            using var registration = cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(true);
                    }
                }
                catch
                {
                    // Ignore exceptions from Kill
                }
            });

            await process.WaitForExitAsync(cancellationToken);

            string output = await outputTask;
            string error = await errorTask;

            return new ProcessResult
            {
                ExitCode = process.ExitCode,
                StandardOutput = output,
                StandardError = error
            };
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(true);
                }
            }
            catch
            {
                // ignore
            }

            return new ProcessResult
            {
                ExitCode = -1,
                StandardOutput = string.Empty,
                StandardError = "Process execution canceled"
            };
        }
    }

    // Convenience: execute with a timeout
    public Task<ProcessResult> ExecuteWithTimeoutAsync(
        string fileName,
        string arguments,
        TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        return ExecuteAsync(fileName, arguments, cts.Token);
    }

    // Synchronous wrapper for callers that need a blocking call
    public ProcessResult Execute(
        string fileName,
        string arguments,
        TimeSpan? timeout = null)
    {
        if (timeout.HasValue)
        {
            return ExecuteWithTimeoutAsync(fileName, arguments, timeout.Value).GetAwaiter().GetResult();
        }

        return ExecuteAsync(fileName, arguments, CancellationToken.None).GetAwaiter().GetResult();
    }
}