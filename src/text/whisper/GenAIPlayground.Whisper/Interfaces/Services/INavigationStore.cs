// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Interfaces.Services;

using GenAIPlayground.Whisper.Interfaces.ViewModels;
using System;

public interface INavigationStore
{
    IViewModel? CurrentViewModel { get; set; }
    event Action? CurrentViewModelChanged;
}