// <copyright file="RepositoryScanScriptBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script;

using System.Text;

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
