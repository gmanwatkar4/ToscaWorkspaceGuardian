using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IWorkspaceHealthAnalyzer
{
    WorkspaceHealthReport Analyze(
        ParsedWorkspace workspace);
}