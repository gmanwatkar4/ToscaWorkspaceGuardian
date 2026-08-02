// <copyright file="ScriptComposer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script;

using System.Text;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe ScriptComposer.

/// </summary>

public class ScriptComposer
{
    private readonly ScriptTemplateRepository repository;

    public ScriptComposer(
        ScriptTemplateRepository repository)
    {
        this.repository = repository;
    }

    public string Compose(ScriptRequest request)
    {
        string template =
            this.repository.GetTemplate(request.ScriptType);

        if (request.ScriptType == ScriptType.WorkspaceAnalysis)
        {
            return template;
        }

        var builder = new StringBuilder();

        foreach (var query in request.Queries)
        {
            builder.AppendLine($"Search \"{query}\" 0");
            builder.AppendLine("Print");
            builder.AppendLine();
        }

        return template.Replace(
            "{SEARCH_BLOCK}",
            builder.ToString());
    }
}

