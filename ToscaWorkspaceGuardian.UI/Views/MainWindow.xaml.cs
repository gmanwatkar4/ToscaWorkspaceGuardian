namespace ToscaWorkspaceGuardian.UI.Views;

using System.Windows;
using System.Windows.Controls;
using ToscaWorkspaceGuardian.UI.ViewModels;

public partial class MainWindow : Window
{
    private ActivityWindow? activityWindow;
    public MainWindow(MainViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel;
        this.Loaded += this.OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs eventArgs)
    {
        var workArea = SystemParameters.WorkArea;
        this.MaxWidth = workArea.Width;
        this.MaxHeight = workArea.Height;
        this.Width = Math.Min(this.Width, workArea.Width);
        this.Height = Math.Min(this.Height, workArea.Height);
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs eventArgs)
    {
        if (this.DataContext is MainViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.Password = passwordBox.Password;
        }
    }

    private void ClientSecretBox_OnPasswordChanged(object sender, RoutedEventArgs eventArgs)
    {
        if (this.DataContext is MainViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.ClientSecret = passwordBox.Password;
        }
    }

    private void OpenActivityMonitor_OnClick(object sender, RoutedEventArgs eventArgs)
    {
        if (this.DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (this.activityWindow is null || !this.activityWindow.IsLoaded)
        {
            this.activityWindow = new ActivityWindow(viewModel) { Owner = this };
            this.activityWindow.Show();
        }
        else
        {
            this.activityWindow.Activate();
        }
    }
}
