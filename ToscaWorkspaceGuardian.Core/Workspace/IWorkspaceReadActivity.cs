namespace ToscaWorkspaceGuardian.Core.Workspace;

/// <summary>Publishes non-sensitive progress messages for a workspace read.</summary>
public interface IWorkspaceReadActivity
{
    event EventHandler<WorkspaceReadActivityEventArgs>? ActivityReported;

    void Report(string message, WorkspaceReadActivityLevel level = WorkspaceReadActivityLevel.Information);
}

/// <summary>One user-visible workspace read activity event.</summary>
public sealed class WorkspaceReadActivityEventArgs : EventArgs
{
    public WorkspaceReadActivityEventArgs(string message, WorkspaceReadActivityLevel level)
    {
        this.Timestamp = DateTimeOffset.Now;
        this.Message = message;
        this.Level = level;
    }

    public DateTimeOffset Timestamp { get; }

    public string Message { get; }

    public WorkspaceReadActivityLevel Level { get; }
}

public enum WorkspaceReadActivityLevel
{
    Information,
    Warning,
    Error,
}
