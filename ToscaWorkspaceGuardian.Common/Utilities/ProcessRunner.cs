// <copyright file="ProcessRunner.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Common.Utilities;

using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Models;

public class ProcessRunner : IProcessRunner
{
    private readonly Microsoft.Extensions.Logging.ILogger<ProcessRunner>? logger;

    public ProcessRunner(Microsoft.Extensions.Logging.ILogger<ProcessRunner>? logger = null)
    {
        this.logger = logger;
    }

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

            CreateNoWindow = true,
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        int maxAttempts = 3;
        int attempt = 0;
        while (true)
        {
            attempt++;
            try
            {
                this.logger?.LogDebug("Starting process {FileName} {Arguments} (attempt {Attempt})", fileName, arguments, attempt);
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
                    catch (Exception ex)
                    {
                        this.logger?.LogWarning(ex, "Failed to kill process on cancellation");
                    }
                });

                await process.WaitForExitAsync(cancellationToken);

                string output = await outputTask;
                string error = await errorTask;

                this.logger?.LogDebug("Process exited with code {ExitCode}", process.ExitCode);

                return new ProcessResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = output,
                    StandardError = error,
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
                catch (Exception ex)
                {
                    this.logger?.LogWarning(ex, "Failed to kill process after cancellation");
                }

                return new ProcessResult
                {
                    ExitCode = -1,
                    StandardOutput = string.Empty,
                    StandardError = "Process execution canceled",
                };
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                this.logger?.LogWarning(ex, "Process execution failed on attempt {Attempt}, retrying...", attempt);

                // small backoff
                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), CancellationToken.None);

                // retry loop
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Process execution failed");
                throw;
            }
        }
    }

    // Convenience: execute with a timeout
    public Task<ProcessResult> ExecuteWithTimeoutAsync(
        string fileName,
        string arguments,
        TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        return this.ExecuteAsync(fileName, arguments, cts.Token);
    }

    // Synchronous wrapper for callers that need a blocking call
    public ProcessResult Execute(
        string fileName,
        string arguments,
        TimeSpan? timeout = null)
    {
        if (timeout.HasValue)
        {
            return this.ExecuteWithTimeoutAsync(fileName, arguments, timeout.Value).GetAwaiter().GetResult();
        }

        return this.ExecuteAsync(fileName, arguments, CancellationToken.None).GetAwaiter().GetResult();
    }
}
