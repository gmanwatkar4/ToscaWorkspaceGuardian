// <copyright file="BatchScriptBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script;

using System.Text;

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
