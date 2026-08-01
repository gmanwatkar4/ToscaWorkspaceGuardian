using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Script.Templates;

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

            ScriptType.Search =>
    """
JumpToProject

{SEARCH_BLOCK}

Exit
""",

            _ => throw new NotSupportedException()
        };
    }
}