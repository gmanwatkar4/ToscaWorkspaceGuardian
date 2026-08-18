namespace ToscaWorkspaceGuardian.Core.Workspace;

/// <summary>In-process activity stream. Messages never contain credentials or command-line arguments.</summary>
public sealed class WorkspaceReadActivity : IWorkspaceReadActivity
{
    public event EventHandler<WorkspaceReadActivityEventArgs>? ActivityReported;

    public void Report(string message, WorkspaceReadActivityLevel level = WorkspaceReadActivityLevel.Information)
    {
        this.ActivityReported?.Invoke(this, new WorkspaceReadActivityEventArgs(message, level));
    }
}
