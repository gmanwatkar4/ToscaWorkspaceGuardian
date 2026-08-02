namespace ToscaWorkspaceGuardian.Common.Interfaces;

using ToscaWorkspaceGuardian.Common.Models;
using System.Threading;

public interface IProcessRunner
{
    Task<ProcessResult> ExecuteAsync(
        string fileName,
        string arguments,
        CancellationToken cancellationToken = default);
}