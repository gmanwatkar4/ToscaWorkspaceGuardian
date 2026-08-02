using System.Text;

namespace ToscaWorkspaceGuardian.Core.Script;

public class BatchScriptBuilder
{
    public string Build(IEnumerable<string> nodePaths)
    {
        var script = new StringBuilder();

        foreach (var path in nodePaths)
        {
            script.AppendLine($"JumpToNode \"{path}\"");
            script.AppendLine();
            script.AppendLine("Print");
            script.AppendLine();
        }

        script.AppendLine("Exit");

        return script.ToString();
    }
}