// <copyright file="IWorkspaceDetector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Models;

public interface IWorkspaceDetector
{
    WorkspaceInfo Detect(string workspacePath);
}
