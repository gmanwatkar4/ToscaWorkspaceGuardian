// <copyright file="WorkspaceAnalysisTemplate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script.Templates;

public static class WorkspaceAnalysisTemplate
{
    public const string Template =
@"// ==========================================
 // Tosca Workspace Guardian
 // Generated Script
 // ==========================================

JumpToProject

Search ""{QUERY}"" 0

Print

Exit";
}
