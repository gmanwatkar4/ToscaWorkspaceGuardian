namespace ToscaWorkspaceGuardian.Tests;

using ToscaWorkspaceGuardian.Core.AI;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TQL;

public sealed class WorkspaceNavigatorServiceTests
{
    [Fact]
    public async Task AskAsync_GeneratesTqlRunsItAndAnswersFromResults()
    {
        var runner = new StubQueryRunner();
        var service = new WorkspaceNavigatorService(new StubAiProvider(), runner);

        var result = await service.AskAsync("Show test cases", new WorkspaceRequest());

        Assert.True(result.Success);
        Assert.Equal("=>SUBPARTS:TestCase", result.GeneratedTql);
        Assert.Equal("=>SUBPARTS:TestCase", runner.LastQuery);
        Assert.Single(result.Results.Objects);
        Assert.Equal("Grounded answer", result.Answer);
    }

    [Fact]
    public void ValidateReadOnlyQuery_RejectsTcshellCommandToken()
    {
        Assert.Throws<InvalidOperationException>(() => WorkspaceNavigatorService.ValidateReadOnlyQuery("=>SUBPARTS:TestCase Task Delete"));
    }

    private sealed class StubAiProvider : IAIProvider
    {
        private int callCount;

        public Task<AIResponse> GenerateAsync(AIRequest request, CancellationToken cancellationToken = default)
        {
            this.callCount++;
            return Task.FromResult(new AIResponse { Content = this.callCount == 1 ? "=>SUBPARTS:TestCase" : "Grounded answer" });
        }
    }

    private sealed class StubQueryRunner : ITqlQueryRunner
    {
        public string LastQuery { get; private set; } = string.Empty;

        public Task<TqlQueryResult> RunAsync(string query, WorkspaceRequest request, CancellationToken cancellationToken = default)
        {
            this.LastQuery = query;
            var document = new OutputDocument();
            document.Objects.Add(new OutputObject { Name = "Checkout", ObjectType = "TestCase" });
            return Task.FromResult(new TqlQueryResult { Success = true, Query = query, Document = document, Message = "OK" });
        }
    }
}
