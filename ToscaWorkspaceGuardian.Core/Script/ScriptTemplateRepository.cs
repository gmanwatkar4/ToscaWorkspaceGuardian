using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Script.Templates;

namespace ToscaWorkspaceGuardian.Core.Script;

public class ScriptTemplateRepository
{
    public string GetTemplate(
        ScriptType scriptType)
    {
        return scriptType switch
        {
            ScriptType.WorkspaceAnalysis
                => WorkspaceAnalysisTemplate.Template,

            _ => throw new NotSupportedException(
                $"Script template '{scriptType}' not found.")
        };
    }
}