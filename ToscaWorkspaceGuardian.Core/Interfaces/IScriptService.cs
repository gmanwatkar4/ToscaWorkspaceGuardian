using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IScriptService
{
    Task<string> GenerateScriptAsync(
        ScriptRequest request,
        CancellationToken cancellationToken = default);
}