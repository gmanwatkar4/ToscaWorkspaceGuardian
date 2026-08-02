using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Script;

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

            _ => throw new NotSupportedException()
        };
    }
}