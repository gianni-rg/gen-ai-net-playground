// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface IAboutViewModel : IViewModel
{
    string ApplicationVersion { get; }
}
