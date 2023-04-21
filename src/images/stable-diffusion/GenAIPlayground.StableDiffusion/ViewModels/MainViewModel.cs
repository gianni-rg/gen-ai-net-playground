// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public partial class MainViewModel : ViewModelBase, IMainViewModel
{
    #region Private fields
    private readonly ILogger _logger;
    private readonly INavigationStore _navigationStore;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    #endregion

    #region Properties
    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _importEnabled;

    [ObservableProperty]
    private bool _exportEnabled;

    public IViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;
    public IViewModel? CurrentModalViewModel => null; // _modalNavigationStore.CurrentViewModel;
    public bool IsDialogOpen => false; // _modalNavigationStore.CurrentViewModel != null;
    #endregion

    #region Constructor
    public MainViewModel()
    {
        // DESIGN TIME //
    }

    public MainViewModel(ILogger logger,
                        IDialogService dialogService,
                        INavigationStore navigationStore,
                        INavigationService navigationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _navigationStore = navigationStore ?? throw new ArgumentNullException(nameof(navigationStore));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    public override void Dispose()
    {
        _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;

        IsActive = false;

        // Unregisters the recipient from all messages
        //WeakReferenceMessenger.Default.Unregister<XXXMessage>(this);
    }
    #endregion

    [RelayCommand]
    private Task ShowAboutDialog() => _dialogService.ShowDialogAsync(nameof(AboutViewModel));

    [RelayCommand]
    private void Exit()
    {
        // TODO: Send CloseRequestMessage
    }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.