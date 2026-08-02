// <copyright file="NodePrintScriptBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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
