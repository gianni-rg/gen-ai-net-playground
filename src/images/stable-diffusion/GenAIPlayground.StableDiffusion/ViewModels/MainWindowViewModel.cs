// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.ViewModels;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.Models;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public partial class MainWindowViewModel : ViewModelBase,
                                           IMainWindowViewModel,
                                           IRecipient<ExitApplicationMessage>

{
    #region Private fields
    private readonly ILogger _logger;
    #endregion

    #region Properties
    public IMainViewModel MainViewModel { get; private set; }

    [ObservableProperty]
    private bool _showOverlay;

    [ObservableProperty]
    private string _windowTitle;
    #endregion

    #region Constructor
    public MainWindowViewModel()
    {
        // DESIGN TIME //
        MainViewModel = new MainViewModel();
        ShowOverlay = true;
        SetWindowTitle();
    }

    public MainWindowViewModel(IMainViewModel mainViewModel, ILogger logger)
    {
        _logger = logger;
        MainViewModel = mainViewModel;
        SetWindowTitle();
    }
    #endregion

    #region Private methods
    protected override void OnActivated()
    {
        base.OnActivated();
        MainViewModel.IsActive = true;
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        MainViewModel.IsActive = false;
    }

    private void SetWindowTitle(string? titleArgs = null)
    {
        titleArgs = !string.IsNullOrWhiteSpace(titleArgs) ? $" [{titleArgs}]" : string.Empty;
        WindowTitle = $"{App.AppName}{titleArgs}";
    }
    #endregion

    #region Message Handlers
    public void Receive(ExitApplicationMessage m)
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;
        lifetime?.MainWindow?.Close();
    }
    #endregion
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.