using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Windows;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TCShell;
using System.Windows;

namespace ToscaWorkspaceGuardian.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IToscaInstallationService _installationService;
    private readonly IWorkspaceDetector _workspaceDetector;
    private readonly IWorkspaceAnalyzer _workspaceAnalyzer;

    public MainViewModel(
    IToscaInstallationService installationService,
    IWorkspaceDetector workspaceDetector,
    IWorkspaceAnalyzer workspaceAnalyzer)
    {
        _installationService = installationService;
        _workspaceDetector = workspaceDetector;
        _workspaceAnalyzer = workspaceAnalyzer;

        var installation = _installationService.GetInstallation();

        StatusMessage = installation.IsInstalled
            ? "Tosca installation detected."
            : "Tosca installation not found.";
    }

    //====================================================
    // Versions
    //====================================================

    [ObservableProperty]
    private string sourceVersion = "2025.1";

    [ObservableProperty]
    private string targetVersion = "2026.1";

    //====================================================
    // Workspace
    //====================================================

    [ObservableProperty]
    private string workspacePath = string.Empty;

    [ObservableProperty]
    private string outputFolder = string.Empty;

    //====================================================
    // Repository
    //====================================================

    [ObservableProperty]
    private string repositoryType = "Not Detected";

    [ObservableProperty]
    private string projectId = string.Empty;

    //====================================================
    // SQL Authentication
    //====================================================

    [ObservableProperty]
    private bool showSqlAuthentication;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    //====================================================
    // TSR Authentication
    //====================================================

    [ObservableProperty]
    private bool showTsrAuthentication;

    [ObservableProperty]
    private string clientId = string.Empty;

    [ObservableProperty]
    private string clientSecret = string.Empty;

    //====================================================
    // Status
    //====================================================

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private bool isAnalyzing;

    //====================================================
    // Browse Workspace
    //====================================================

    [RelayCommand]
    private void BrowseWorkspace()
    {
        var dialog = new OpenFileDialog();

        dialog.Filter =
            "Tosca Workspace (*.tws)|*.tws";

        if (dialog.ShowDialog() != true)
            return;

        WorkspacePath = dialog.FileName;

        DetectWorkspace();
    }

    //====================================================
    // Browse Output Folder
    //====================================================

    [RelayCommand]
    private void BrowseOutputFolder()
    {
        var dialog = new OpenFolderDialog();

        if (dialog.ShowDialog() == true)
        {
            OutputFolder = dialog.FolderName;
        }
    }

    //====================================================
    // Detect Workspace
    //====================================================

    private void DetectWorkspace()
    {
        var info =
            _workspaceDetector.Detect(WorkspacePath);

        RepositoryType = info.RepositoryType;

        ProjectId = info.ProjectId ?? "";

        Username = info.DefaultUser ?? "";

        ShowSqlAuthentication =
            info.RequiresUserPassword;

        ShowTsrAuthentication =
            info.RequiresClientSecret;

        StatusMessage =
            $"Repository detected : {RepositoryType}";
    }

    //====================================================
    // Analyze
    //====================================================


    [RelayCommand]


    private async Task Analyze()
    {

        if (string.IsNullOrWhiteSpace(WorkspacePath))
        {
            StatusMessage = "Please select a workspace.";
            return;
        }

        IsAnalyzing = true;

        try
        {
            var request = new WorkspaceRequest
            {
                WorkspacePath = WorkspacePath,
                Username = Username,
                Password = Password,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                IsManagedRepository = ShowTsrAuthentication
            };

            var result = await _workspaceAnalyzer.AnalyzeAsync(request);

            StatusMessage = result.Success
                ? result.Message
                : "Workspace analysis failed.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsAnalyzing = false;
        }
    }
}