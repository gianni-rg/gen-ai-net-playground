namespace GenAIPlayground.StableDiffusion.Interfaces.Services;

using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using System;

public interface INavigationStore
{
    IViewModel? CurrentViewModel { get; set; }
    event Action? CurrentViewModelChanged;
}