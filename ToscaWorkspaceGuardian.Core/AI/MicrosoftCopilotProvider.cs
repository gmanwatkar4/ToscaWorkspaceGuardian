using System.Threading;

namespace ToscaWorkspaceGuardian.Core.AI;

public class MicrosoftCopilotProvider : IAIProvider
{
    public Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}