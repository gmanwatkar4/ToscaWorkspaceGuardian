using System.Text;

namespace ToscaWorkspaceGuardian.Core.Script;

public class RepositoryScanScriptBuilder
{
    public string Build(IEnumerable<string> queries)
    {
        var script = new StringBuilder();

        script.AppendLine("JumpToProject");
        script.AppendLine();

        foreach (var query in queries)
        {
            script.AppendLine(
                $"For \"{query}\" CallOnEach PrintObject.tcs");

            script.AppendLine();
        }

        script.AppendLine("Exit");

        return script.ToString();
    }
}