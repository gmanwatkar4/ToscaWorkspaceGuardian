namespace ToscaWorkspaceGuardian.Common.Interfaces;

using ToscaWorkspaceGuardian.Common.Models;

public interface IProcessRunner
{
    Task<ProcessResult> ExecuteAsync(
        string fileName,
        string arguments);
}