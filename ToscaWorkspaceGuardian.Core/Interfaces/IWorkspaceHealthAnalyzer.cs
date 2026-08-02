// <copyright file="IWorkspaceHealthAnalyzer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

public interface IWorkspaceHealthAnalyzer
{
    WorkspaceHealthReport Analyze(
        ParsedWorkspace workspace);
}
