// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Services;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.Models.Dialog;
using GenAIPlayground.StableDiffusion.ViewModels;
using GenAIPlayground.StableDiffusion.Views;
using Splat;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GenAIPlayground.StableDiffusion.DependencyInjection;

public class DialogService : IDialogService
{
    private readonly IReadonlyDependencyResolver _resolver;

    public DialogService(IReadonlyDependencyResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<TResult> ShowDialogAsync<TResult>(string viewModelName)
        where TResult : DialogResultBase
    {
        var window = CreateView<TResult>(viewModelName);
        var viewModel = CreateViewModel<TResult>(viewModelName);
        Bind(window, viewModel);
        viewModel.IsActive = true;
        return await ShowDialogAsync(window);
    }

    public async Task<TResult> ShowDialogAsync<TResult, TParameter>(string viewModelName, TParameter parameter)
        where TResult : DialogResultBase
        where TParameter : NavigationParameterBase
    {
        var window = CreateView<TResult>(viewModelName);
        var viewModel = CreateViewModel<TResult>(viewModelName);
        Bind(window, viewModel);

        switch (viewModel)
        {
            case ParameterizedDialogViewModelBase<TResult, TParameter> parameterizedDialogViewModelBase:
                parameterizedDialogViewModelBase.Activate(parameter);
                break;
            case ParameterizedDialogViewModelBaseAsync<TResult, TParameter> parameterizedDialogViewModelBaseAsync:
                await parameterizedDialogViewModelBaseAsync.ActivateAsync(parameter);
                break;
            default:
                throw new InvalidOperationException($"{viewModel.GetType().FullName} doesn't support passing parameters!");
        }

        return await ShowDialogAsync(window);
    }

    public Task ShowDialogAsync(string viewModelName) => ShowDialogAsync<DialogResultBase>(viewModelName);

    public Task ShowDialogAsync<TParameter>(string viewModelName, TParameter parameter)
        where TParameter : NavigationParameterBase =>
        ShowDialogAsync<DialogResultBase, TParameter>(viewModelName, parameter);

    private static void Bind(IDataContextProvider window, object viewModel) => window.DataContext = viewModel;

    private static DialogWindowBase<TResult> CreateView<TResult>(string viewModelName)
        where TResult : DialogResultBase
    {
        var viewType = GetViewType(viewModelName);
        if (viewType is null)
        {
            throw new InvalidOperationException($"View for {viewModelName} was not found!");
        }

        return GetView<DialogWindowBase<TResult>>(viewType) ?? throw new InvalidOperationException($"Unable to create a view instance for {viewModelName}");
    }

    private DialogViewModelBase<TResult> CreateViewModel<TResult>(string viewModelName)
        where TResult : DialogResultBase
    {
        var viewModelType = GetViewModelType(viewModelName);
        if (viewModelType is null)
        {
            throw new InvalidOperationException($"View model {viewModelName} was not found!");
        }

        return GetViewModel<DialogViewModelBase<TResult>>(viewModelType) ?? throw new InvalidOperationException($"Unable to create a ViewModel instance for {viewModelName}");
    }

    private static Type? GetViewModelType(string viewModelName)
    {
        var viewModelsAssembly = Assembly.GetAssembly(typeof(ViewModelBase));
        if (viewModelsAssembly is null)
        {
            throw new InvalidOperationException("Broken installation!");
        }

        var viewModelTypes = viewModelsAssembly.GetTypes();
        var viewModelInterface = $"I{viewModelName}";
        return viewModelTypes.SingleOrDefault(t => t.Name == viewModelInterface);
    }

    private static TInstanceType? GetView<TInstanceType>(Type type) => (TInstanceType?)Activator.CreateInstance(type);

    private TInstanceType GetViewModel<TInstanceType>(Type type) => (TInstanceType)_resolver.GetRequiredService(type);

    private static Type? GetViewType(string viewModelName)
    {
        var viewsAssembly = Assembly.GetExecutingAssembly();
        var viewTypes = viewsAssembly.GetTypes();
        var viewName = viewModelName.Replace("ViewModel", "View");

        return viewTypes.SingleOrDefault(t => t.Name == viewName);
    }

    private async Task<TResult> ShowDialogAsync<TResult>(DialogWindowBase<TResult> window)
        where TResult : DialogResultBase
    {

        var mainWindow = GetMainWindow();
        var mainWindowDataContext = (IMainWindowViewModel?)mainWindow?.DataContext;

        if (mainWindow is null)
        {
            throw new NullReferenceException(nameof(mainWindow));
        }

        if (mainWindowDataContext is null)
        {
            throw new NullReferenceException(nameof(mainWindowDataContext));
        }

        // Should be implemented via Messages, so that MainWindow can handle Show/Hide cleanly
        mainWindowDataContext.ShowOverlay = true;
        var result = await window.ShowDialog<TResult>(mainWindow);

        mainWindowDataContext.ShowOverlay = false;
        if (window is IDisposable disposable)
        {
            disposable.Dispose();
        }

        return result;
    }

    private Window? GetMainWindow()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;

        return lifetime?.MainWindow;
    }
}