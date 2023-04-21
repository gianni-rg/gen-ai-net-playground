// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Services;

using GenAIPlayground.Whisper.Interfaces.Services;
using GenAIPlayground.Whisper.Interfaces.ViewModels;
using System;

public class NavigationStore : INavigationStore
{
    private IViewModel? _currentViewModel;
    public IViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
