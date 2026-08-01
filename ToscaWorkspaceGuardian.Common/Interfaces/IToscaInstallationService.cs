using ToscaWorkspaceGuardian.Common.Models;

namespace ToscaWorkspaceGuardian.Common.Interfaces;

public interface IToscaInstallationService
{
    ToscaInstallation GetInstallation();
}