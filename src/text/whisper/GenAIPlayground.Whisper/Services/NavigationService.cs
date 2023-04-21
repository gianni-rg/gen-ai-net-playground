// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Services;

using GenAIPlayground.Whisper.DependencyInjection;
using GenAIPlayground.Whisper.Interfaces.Services;
using GenAIPlayground.Whisper.Interfaces.ViewModels;
using Splat;
using System;
using System.Collections.Generic;

public class NavigationService : INavigationService
{
    private readonly IReadonlyDependencyResolver _resolver;
    private readonly INavigationStore _navigationStore;
    private readonly Stack<IViewModel> _navigationHistory;

    public bool CanNavigateBack => _navigationHistory.Count > 0;

    public NavigationService(IReadonlyDependencyResolver resolver, INavigationStore navigationStore /*, INavigationStore modalNavigationStore*/)
    {
        _resolver = resolver;
        _navigationStore = navigationStore;
        _navigationHistory = new Stack<IViewModel>();
    }

    public void NavigateTo<TViewModel>(object? parameter = default, bool canNavigateBack = false)
        where TViewModel : IViewModel
    {
        if (_navigationStore.CurrentViewModel is not null)
        {
            _navigationStore.CurrentViewModel.IsActive = false;
            if (canNavigateBack)
            {
                _navigationHistory.Push(_navigationStore.CurrentViewModel);
            }
            else
            {
                _navigationStore.CurrentViewModel.Dispose();
            }
        }

        IViewModel viewModel = CreateViewModel<TViewModel>();
        viewModel.Parameter = parameter;

        _navigationStore.CurrentViewModel = viewModel;
        _navigationStore.CurrentViewModel.IsActive = true;
    }

    public void NavigateBack(object? parameter = default)
    {
        if (!CanNavigateBack)
        {
            return;
        }

        if (_navigationStore.CurrentViewModel is not null)
        {
            _navigationStore.CurrentViewModel.IsActive = false;
            _navigationStore.CurrentViewModel.Dispose();
        }

        IViewModel viewModel = _navigationHistory.Pop();
        viewModel.Parameter = parameter;

        _navigationStore.CurrentViewModel = viewModel;
        _navigationStore.CurrentViewModel.IsActive = true;
    }

    private TViewModel CreateViewModel<TViewModel>()
    {
        return _resolver.GetRequiredService<TViewModel>() ?? throw new InvalidOperationException($"Unable to create a ViewModel instance for {typeof(TViewModel)}");
    }
}