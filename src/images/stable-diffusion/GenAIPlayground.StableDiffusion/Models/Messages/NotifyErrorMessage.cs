// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Models;

using GenAIPlayground.StableDiffusion.Models.Dialog;

public class NotifyErrorMessage : NavigationParameterBase
{
    public string Message { get; }

    public NotifyErrorMessage(string message)
    {
        Message = message;
    }
}