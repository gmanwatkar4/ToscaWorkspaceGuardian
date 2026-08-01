using System.IO;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Models;

namespace ToscaWorkspaceGuardian.Common.Services;

public class ToscaInstallationService : IToscaInstallationService
{
    public ToscaInstallation GetInstallation()
    {
        var installation = new ToscaInstallation();

        string[] possiblePaths =
        {
            @"C:\Program Files\TRICENTIS\Tosca Testsuite",
            @"C:\Program Files (x86)\TRICENTIS\Tosca Testsuite"
        };

        foreach (var path in possiblePaths)
        {
            if (!Directory.Exists(path))
                continue;

            installation.InstallationPath = path;

            installation.TCShellPath =
                Path.Combine(path, "ToscaCommander", "TCShell.exe");

            installation.ToscaCommanderPath =
                Path.Combine(path, "ToscaCommander", "ToscaCommander.exe");

            installation.TBoxPath =
                Path.Combine(path, "TBox", "TBox.exe");

            installation.IsInstalled =
                File.Exists(installation.TCShellPath);

            if (installation.IsInstalled)
                return installation;
        }

        return installation;
    }
}