// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface IErrorMessageViewModel : IViewModel
{
    string Message { get; }
}
