// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface IMainViewModel : IViewModel
{
    IViewModel? CurrentViewModel { get; }
    IViewModel? CurrentModalViewModel { get; }
    bool IsDialogOpen { get; }
    string StatusMessage { get; }
    bool IsBusy { get; }
}
