// <copyright file="ScriptTemplateRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script;

using ToscaWorkspaceGuardian.Core.Models;

public class ScriptTemplateRepository
{
    public string GetTemplate(ScriptType type)
    {
        return type switch
        {
            ScriptType.WorkspaceAnalysis =>
"""
JumpToProject

Print

Exit
""",

            ScriptType.RepositoryScan =>
"""
JumpToProject

{SEARCH_BLOCK}

Exit
""",

            _ => throw new NotSupportedException(),
        };
    }
}
