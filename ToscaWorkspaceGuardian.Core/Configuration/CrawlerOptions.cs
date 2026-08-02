namespace ToscaWorkspaceGuardian.Core.Configuration;

public class CrawlerOptions
{
    // Maximum number of batches to process in parallel
    public int MaxDegreeOfParallelism { get; set; } = 4;

    // Maximum number of paths per batch
    public int BatchSize { get; set; } = 50;
}
