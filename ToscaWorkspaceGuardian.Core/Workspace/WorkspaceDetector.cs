// <copyright file="WorkspaceDetector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Workspace;

using System.Xml.Linq;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe WorkspaceDetector.

/// </summary>

public class WorkspaceDetector : IWorkspaceDetector
{
    public WorkspaceInfo Detect(string workspacePath)
    {
        var info = new WorkspaceInfo();

        info.WorkspacePath = workspacePath;

        if (!File.Exists(workspacePath))
        {
            return info;
        }

        var document = XDocument.Load(workspacePath);

        var commonRepository =
            document.Root?.Element("CommonRepository");

        if (commonRepository == null)
        {
            return info;
        }

        string repoClass =
            commonRepository.Element("RepoClass")?.Value ?? string.Empty;

        info.RepositoryType = repoClass;

        switch (repoClass)
        {
            case "ManagedRepository":

                info.IsManagedRepository = true;

                info.RequiresClientSecret = true;

                info.ProjectId =
                    commonRepository.Element("ProjectId")?.Value;

                break;

            case "MSSQLRepository":

            case "OracleRepository":

            case "DB2Repository":

                info.RequiresUserPassword = true;

                info.ConnectionData =
                    commonRepository.Element("ConnectionData")?.Value;

                break;
        }

        info.DefaultUser =
            document.Root?
                    .Element("DefaultUser")?
                    .Element("UserName")?
                    .Value;

        return info;
    }
}

