// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Views;

using Avalonia;
using Avalonia.Controls;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closing += MainWindow_Closing;
        Opened += MainWindow_Opened;
        Closed += MainWindow_Closed;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    #region Window Event Handlers
    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var mainWindowViewModel = (IMainWindowViewModel?)DataContext;
        mainWindowViewModel?.MainViewModel.ExitCommand.Execute("true");
    }

    private void MainWindow_Closed(object? sender, System.EventArgs e)
    {
        var mainWindowViewModel = (IMainWindowViewModel?)DataContext;
        if (mainWindowViewModel is null)
        {
            return;
        }

        mainWindowViewModel.IsActive = false;
    }

    private void MainWindow_Opened(object? sender, System.EventArgs e)
    {
        var mainWindowViewModel = (IMainWindowViewModel?)DataContext;
        if (mainWindowViewModel is null)
        {
            return;
        }

        mainWindowViewModel.IsActive = true;
    } 
    #endregion
}