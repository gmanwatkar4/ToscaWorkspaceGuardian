namespace ToscaWorkspaceGuardian.Core.TCShell;

public interface ITCShellPreflightService
{
    Task<TCShellPreflightResult> CheckAsync(CancellationToken cancellationToken = default);
}
