// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface IViewModel
{
    object? Parameter { get; set; }
    bool IsActive { get; set; }
    void Dispose();
}
