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