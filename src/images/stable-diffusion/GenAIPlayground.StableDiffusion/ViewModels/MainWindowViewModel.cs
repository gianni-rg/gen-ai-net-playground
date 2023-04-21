// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public partial class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    private readonly ILogger _logger;

    public IMainViewModel MainViewModel { get; private set; }

    [ObservableProperty]
    private bool _showOverlay;

    [ObservableProperty]
    private string _windowTitle;

    public MainWindowViewModel()
    {
        MainViewModel = new MainViewModel();
        SetWindowTitle();
    }

    public MainWindowViewModel(IMainViewModel mainViewModel, ILogger logger)
    {
        _logger = logger;
        MainViewModel = mainViewModel;
        SetWindowTitle();
    }

    private void SetWindowTitle(string? titleArgs = null)
    {
        titleArgs = !string.IsNullOrWhiteSpace(titleArgs) ? $" [{titleArgs}]" : string.Empty;
        WindowTitle = $"{App.AppName}{titleArgs}";
    }

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
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.