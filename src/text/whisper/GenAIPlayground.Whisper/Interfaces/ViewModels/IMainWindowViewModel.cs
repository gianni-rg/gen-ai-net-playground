// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Interfaces.ViewModels;

public interface IMainWindowViewModel : IViewModel
{
    IMainViewModel MainViewModel { get; }
    bool ShowOverlay { get; set; }
    string WindowTitle { get; set; }
}
