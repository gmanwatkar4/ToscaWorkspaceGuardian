// <copyright file="IParsedWorkspaceMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe IParsedWorkspaceMapper.

/// </summary>

public interface IParsedWorkspaceMapper
{
    ParsedWorkspace Map(OutputDocument document);
}

