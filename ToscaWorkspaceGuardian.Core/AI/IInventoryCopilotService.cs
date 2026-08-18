namespace ToscaWorkspaceGuardian.Core.AI;

public interface IInventoryCopilotService
{
    Task<InventoryCopilotResponse> AskAsync(string inventoryFilePath, string question, CancellationToken cancellationToken = default);
}

public sealed class InventoryCopilotResponse
{
    public string Answer { get; init; } = string.Empty;

    public string EvidenceSummary { get; init; } = string.Empty;
}
