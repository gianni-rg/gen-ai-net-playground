// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Models;

public class UpdateStatusBarMessage
{
    public string Text { get; }

    public UpdateStatusBarMessage(string text)
    {
        Text = text;
    }
}
