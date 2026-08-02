// <copyright file="ISnapshotBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe ISnapshotBuilder.

/// </summary>

public interface ISnapshotBuilder
{
    void AddDocument(
        WorkspaceSnapshot snapshot,
        OutputDocument document);
}

