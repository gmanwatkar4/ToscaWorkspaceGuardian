// <copyright file="MainViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.UI.ViewModels;

using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TCShell;

/// <summary>

/// TODO: Describe MainViewModel.

/// </summary>

public partial class MainViewModel : ObservableObject
{
    private readonly IToscaInstallationService installationService;
    private readonly IWorkspaceDetector workspaceDetector;
    private readonly IWorkspaceAnalyzer workspaceAnalyzer;
    private readonly IWorkspaceSnapshotExporter snapshotExporter;

    public MainViewModel(
    IToscaInstallationService installationService,
    IWorkspaceSnapshotExporter snapshotExporter,
    IWorkspaceDetector workspaceDetector,
    IWorkspaceAnalyzer workspaceAnalyzer)
    {
        this.installationService = installationService;
        this.workspaceDetector = workspaceDetector;
        this.workspaceAnalyzer = workspaceAnalyzer;
        this.snapshotExporter = snapshotExporter;

        var installation = this.installationService.GetInstallation();

        this.StatusMessage = installation.IsInstalled
            ? "Tosca installation detected."
            : "Tosca installation not found.";
    }

    // ====================================================
    // Versions
    // ====================================================
    [ObservableProperty]
    private string sourceVersion = "2025.1";

    [ObservableProperty]
    private string targetVersion = "2026.1";

    // ====================================================
    // Workspace
    // ====================================================
    [ObservableProperty]
    private string workspacePath = string.Empty;

    [ObservableProperty]
    private string outputFolder = string.Empty;

    // ====================================================
    // Repository
    // ====================================================
    [ObservableProperty]
    private string repositoryType = "Not Detected";

    [ObservableProperty]
    private string projectId = string.Empty;

    // ====================================================
    // SQL Authentication
    // ====================================================
    [ObservableProperty]
    private bool showSqlAuthentication;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    // ====================================================
    // TSR Authentication
    // ====================================================
    [ObservableProperty]
    private bool showTsrAuthentication;

    [ObservableProperty]
    private string clientId = string.Empty;

    [ObservableProperty]
    private string clientSecret = string.Empty;

    // ====================================================
    // Status
    // ====================================================
    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private bool isAnalyzing;

    // ====================================================
    // Browse Workspace
    // ====================================================
    [RelayCommand]
    private void BrowseWorkspace()
    {
        var dialog = new OpenFileDialog();

        dialog.Filter =
            "Tosca Workspace (*.tws)|*.tws";

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        this.WorkspacePath = dialog.FileName;

        this.DetectWorkspace();
    }

    // ====================================================
    // Browse Output Folder
    // ====================================================
    [RelayCommand]
    private void BrowseOutputFolder()
    {
        var dialog = new OpenFolderDialog();

        if (dialog.ShowDialog() == true)
        {
            this.OutputFolder = dialog.FolderName;
        }
    }

    // ====================================================
    // Detect Workspace
    // ====================================================
    private void DetectWorkspace()
    {
        var info =
            this.workspaceDetector.Detect(this.WorkspacePath);

        this.RepositoryType = info.RepositoryType;

        this.ProjectId = info.ProjectId ?? string.Empty;

        this.Username = info.DefaultUser ?? string.Empty;

        this.ShowSqlAuthentication =
            info.RequiresUserPassword;

        this.ShowTsrAuthentication =
            info.RequiresClientSecret;

        this.StatusMessage =
            $"Repository detected : {this.RepositoryType}";
    }

    // ====================================================
    // Analyze
    // ====================================================
    [RelayCommand]

    private async Task Analyze()
    {
        if (string.IsNullOrWhiteSpace(this.WorkspacePath))
        {
            this.StatusMessage = "Please select a workspace.";
            return;
        }

        this.IsAnalyzing = true;

        try
        {
            var request = new WorkspaceRequest
            {
                WorkspacePath = this.WorkspacePath,
                Username = this.Username,
                Password = this.Password,
                ClientId = this.ClientId,
                ClientSecret = this.ClientSecret,
                IsManagedRepository = this.ShowTsrAuthentication,
                SourceVersion = this.SourceVersion,
                TargetVersion = this.TargetVersion,
            };

            var result = await this.workspaceAnalyzer.AnalyzeAsync(request);

            this.StatusMessage = result.Success
                ? result.Message
                : "Workspace analysis failed.";
        }
        catch (Exception ex)
        {
            this.StatusMessage = ex.Message;
        }
        finally
        {
            this.IsAnalyzing = false;
        }
    }
}

