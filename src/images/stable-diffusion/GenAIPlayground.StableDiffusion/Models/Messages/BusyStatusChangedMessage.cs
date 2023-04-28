// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Models;

public class BusyStatusChangedMessage
{
    public bool IsBusy { get; }
    public bool ShowOverlay { get; }

    public BusyStatusChangedMessage(bool isBusy, bool showOverlay = false)
    {
        IsBusy = isBusy;
        ShowOverlay = showOverlay;
    }
}