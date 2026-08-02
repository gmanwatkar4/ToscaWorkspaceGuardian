// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.UI.Views;

using System.Windows;
using ToscaWorkspaceGuardian.UI.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        this.InitializeComponent();

        this.DataContext = viewModel;
    }
}
