// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Interfaces.Services;

using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.Models.Dialog;
using System.Threading.Tasks;

public interface IDialogService
{
    Task<TResult> ShowDialogAsync<TResult, TViewModel>()
        where TViewModel : IViewModel
        where TResult : DialogResultBase;

    Task ShowDialogAsync<TViewModel>()
        where TViewModel : IViewModel;

    Task ShowDialogAsync<TViewModel, TParameter>(TParameter parameter)
        where TViewModel : IViewModel
        where TParameter : NavigationParameterBase;

    Task<TResult> ShowDialogAsync<TResult, TViewModel, TParameter>(TParameter parameter)
        where TResult : DialogResultBase
        where TViewModel : IViewModel
        where TParameter : NavigationParameterBase;
}