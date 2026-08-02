namespace ToscaWorkspaceGuardian.Core.Business;

public static class SnapshotPaths
{
    public static string Default =>
        Path.Combine(
            Path.GetTempPath(),
            "ToscaWorkspaceGuardian",
            "WorkspaceSnapshot.json");
}