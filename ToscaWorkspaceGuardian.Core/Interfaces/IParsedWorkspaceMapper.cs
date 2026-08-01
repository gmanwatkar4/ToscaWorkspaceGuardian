using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IParsedWorkspaceMapper
{
    ParsedWorkspace Map(OutputDocument document);
}