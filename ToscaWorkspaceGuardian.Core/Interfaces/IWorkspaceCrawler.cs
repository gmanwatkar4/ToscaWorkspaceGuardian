using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IWorkspaceCrawler
{
    Task<WorkspaceSnapshot> CrawlAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}