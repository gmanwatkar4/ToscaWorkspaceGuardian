// <copyright file="NodePrintScriptBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script;

/// <summary>

/// TODO: Describe NodePrintScriptBuilder.

/// </summary>

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

