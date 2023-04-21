// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Interfaces.Services;

using GenAIPlayground.Whisper.Interfaces.ViewModels;

public interface INavigationService
{
    bool CanNavigateBack { get; }
    void NavigateTo<TViewModel>(object? parameter = default, bool canNavigateBack = false)
        where TViewModel : IViewModel;
    void NavigateBack(object? parameter = default);
}