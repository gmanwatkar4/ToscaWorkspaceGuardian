using System.Text;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Script;

public class ScriptComposer
{
    private readonly ScriptTemplateRepository _repository;

    public ScriptComposer(
        ScriptTemplateRepository repository)
    {
        _repository = repository;
    }

    public string Compose(
        ScriptRequest request)
    {
        string template =
            _repository.GetTemplate(request.ScriptType);

        if (request.ScriptType == ScriptType.WorkspaceAnalysis)
            return template;

        var builder = new StringBuilder();

        foreach (var query in request.Queries)
        {
            builder.AppendLine($"Search \"{query}\" 0");
            builder.AppendLine();
            builder.AppendLine("Print");
            builder.AppendLine();
        }

        return template.Replace(
            "{SEARCH_BLOCK}",
            builder.ToString());
    }
}