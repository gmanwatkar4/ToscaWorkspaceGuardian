namespace ToscaWorkspaceGuardian.Core.Script;

public class NodePrintScriptBuilder
{
    public string Build(string nodePath)
    {
        return
$"""
JumpToNode "{nodePath}"

Print

Exit
""";
    }
}