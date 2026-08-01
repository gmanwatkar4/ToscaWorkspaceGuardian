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
            _repository.GetTemplate(
                request.ScriptType);

        return template.Replace(
            "{QUERY}",
            request.Query);
    }
}