namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text;
using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Workspace;

/// <summary>Answers questions from the saved inventory and supplies only relevant, traceable evidence to AI.</summary>
public sealed class InventoryCopilotService : IInventoryCopilotService
{
    private readonly IAIProvider aiProvider;
    private readonly List<ConversationTurn> conversation = [];
    private string? conversationInventoryPath;

    public InventoryCopilotService(IAIProvider aiProvider) => this.aiProvider = aiProvider;

    public async Task<InventoryCopilotResponse> AskAsync(string inventoryFilePath, string question, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(question);
        var inventory = JsonSerializer.Deserialize<WorkspaceInventorySnapshot>(
            await File.ReadAllTextAsync(inventoryFilePath, cancellationToken),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("The workspace inventory JSON could not be read.");

        var evidence = BuildEvidence(inventory, question);
        var priorConversation = this.GetPriorConversation(inventoryFilePath);
        var prompt = $"""
            You are the Tosca Upgrade Guide's friendly Inventory AI. Have a natural, helpful conversation like an experienced Tosca support engineer.
            Reply in the same language and tone as the user. If the user writes Hinglish, reply in clear Hinglish.
            Answer directly first, then add a short explanation of what the number or finding means. Use normal sentences and small bullets only when they improve clarity.
            Do not use rigid headings such as "Result", "Evidence", or "Next step" unless the user explicitly asks for a formal report.
            Answer only from the saved workspace inventory evidence below. Never invent missing workspace objects or claim a module is broken.
            Treat duplicates and unused modules as candidates unless direct references prove otherwise. Mention an exact NodePath only when it helps answer the question.

            Question: {question}

            Previous conversation (may be empty):
            {priorConversation}

            Inventory evidence:
            {evidence}
            """;
        var ai = await this.aiProvider.GenerateAsync(new AIRequest { Prompt = prompt }, cancellationToken);
        var answer = string.IsNullOrWhiteSpace(ai.Content) ? BuildFallback(inventory, evidence) : ai.Content.Trim();
        this.RememberTurn(inventoryFilePath, question, answer);
        return new InventoryCopilotResponse
        {
            Answer = answer,
            EvidenceSummary = evidence,
        };
    }

    private static string BuildEvidence(WorkspaceInventorySnapshot inventory, string question)
    {
        var modules = inventory.Nodes.Where(node => string.Equals(node.ObjectType, "XModule", StringComparison.OrdinalIgnoreCase)).ToList();
        var steps = inventory.Nodes.Where(node => node.ObjectType.Contains("TestStep", StringComparison.OrdinalIgnoreCase)).ToList();
        var stepText = string.Join("\n", steps.Select(node => string.Join(" ", node.Properties.Values.Concat(node.Collections.Values.SelectMany(value => value)))));
        var linked = modules.Where(module => !string.IsNullOrWhiteSpace(module.UniqueId) && stepText.Contains(module.UniqueId, StringComparison.OrdinalIgnoreCase)).ToList();
        var duplicates = modules.Where(module => !string.IsNullOrWhiteSpace(module.Name)).GroupBy(module => module.Name, StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1).Take(12).ToList();
        var queryTerms = question.Split(new[] { ' ', '?', ',', '.', ':' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var matches = inventory.Nodes.Where(node => queryTerms.Any(term => term.Length > 2 && (node.Name.Contains(term, StringComparison.OrdinalIgnoreCase) || node.NodePath.Contains(term, StringComparison.OrdinalIgnoreCase) || node.Properties.Values.Any(value => value.Contains(term, StringComparison.OrdinalIgnoreCase))))).Take(15).ToList();
        var builder = new StringBuilder();
        builder.AppendLine($"Inventory: {inventory.ObjectCount} nodes; {modules.Count} XModules; {steps.Count} TestStep-like nodes.");
        builder.AppendLine($"Direct module-ID references found in scanned TestStep data: {linked.Count}. Unlinked-module candidates: {modules.Count - linked.Count}.");
        builder.AppendLine("Duplicate module-name candidates:");
        foreach (var group in duplicates) builder.AppendLine($"- {group.Key}: {group.Count()} modules at {string.Join(" | ", group.Select(item => item.NodePath))}");
        builder.AppendLine("Question-matched inventory evidence:");
        foreach (var node in matches) builder.AppendLine($"- {node.ObjectType} | {node.Name} | {node.NodePath} | ID={node.UniqueId}");
        return builder.ToString();
    }

    private string GetPriorConversation(string inventoryFilePath)
    {
        lock (this.conversation)
        {
            if (!string.Equals(this.conversationInventoryPath, inventoryFilePath, StringComparison.OrdinalIgnoreCase))
            {
                this.conversation.Clear();
                this.conversationInventoryPath = inventoryFilePath;
            }

            return this.conversation.Count == 0
                ? "No earlier question in this inventory session."
                : string.Join("\n", this.conversation.TakeLast(3).Select(turn => $"User: {turn.Question}\nAssistant: {turn.Answer}"));
        }
    }

    private void RememberTurn(string inventoryFilePath, string question, string answer)
    {
        lock (this.conversation)
        {
            this.conversationInventoryPath = inventoryFilePath;
            this.conversation.Add(new ConversationTurn(question, answer));
            if (this.conversation.Count > 3)
            {
                this.conversation.RemoveAt(0);
            }
        }
    }

    private static string BuildFallback(WorkspaceInventorySnapshot inventory, string evidence) =>
        $"Maine saved inventory check kiya: is workspace mein {inventory.ObjectCount} nodes detect hue hain. " +
        "AI provider ka response abhi available nahi mila, isliye neeche verified JSON summary di hai:\n\n" + evidence;

    private sealed record ConversationTurn(string Question, string Answer);
}
