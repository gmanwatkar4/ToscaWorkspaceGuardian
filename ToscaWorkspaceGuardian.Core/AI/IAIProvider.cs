namespace ToscaWorkspaceGuardian.Core.AI;

public interface IAIProvider
{
    Task<AIResponse> GenerateAsync(
        AIRequest request,
        CancellationToken cancellationToken = default);
}