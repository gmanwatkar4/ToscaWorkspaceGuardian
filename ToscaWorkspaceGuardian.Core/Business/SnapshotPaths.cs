// <copyright file="SnapshotPaths.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Business;

public static class SnapshotPaths
{
    public static string Default =>
        Path.Combine(
            Path.GetTempPath(),
            "ToscaWorkspaceGuardian",
            "WorkspaceSnapshot.json");
}
