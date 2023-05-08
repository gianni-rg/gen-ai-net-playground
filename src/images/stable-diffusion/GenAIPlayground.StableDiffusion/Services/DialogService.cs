// Copyright (C) 2023 Gianni Rosa Gallina. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License").
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace GenAIPlayground.StableDiffusion.Services;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using GenAIPlayground.StableDiffusion.DependencyInjection;
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

public class DialogService : IDialogService
{
    #region Private fields
    private readonly IReadonlyDependencyResolver _resolver;
    #endregion

    #region Constructor
    public DialogService(IReadonlyDependencyResolver resolver)
    {
        _resolver = resolver;
    }
    #endregion

    #region Public methods
    public async Task<TResult> ShowDialogAsync<TResult, TViewModel>()
        where TViewModel : IViewModel
        where TResult : DialogResultBase
    {
        var window = CreateView<TResult, TViewModel>();
        var viewModel = CreateViewModel<TResult, TViewModel>();
        Bind(window, viewModel);
        viewModel.IsActive = true;
        return await ShowDialogAsync(window);
    }

    public async Task<TResult> ShowDialogAsync<TResult, TViewModel, TParameter>(TParameter parameter)
        where TResult : DialogResultBase
        where TViewModel : IViewModel
        where TParameter : NavigationParameterBase
    {
        var window = CreateView<TResult, TViewModel>();
        var viewModel = CreateViewModel<TResult, TViewModel>();
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

    public Task ShowDialogAsync<TViewModel>()
        where TViewModel : IViewModel 
        => ShowDialogAsync<DialogResultBase, TViewModel>();

    public Task ShowDialogAsync<TViewModel, TParameter>(TParameter parameter)
        where TViewModel : IViewModel
        where TParameter : NavigationParameterBase =>
        ShowDialogAsync<DialogResultBase, TViewModel, TParameter>(parameter);

    #endregion

    #region Private methods
    private static void Bind(IDataContextProvider window, object viewModel) => window.DataContext = viewModel;

    private static DialogWindowBase<TResult> CreateView<TResult, TViewModel>()
        where TViewModel : IViewModel
        where TResult : DialogResultBase
    {
        var viewModelName = typeof(TViewModel).Name;
        var viewType = GetViewType(viewModelName) ?? throw new InvalidOperationException($"View for {viewModelName} was not found!");
        return GetView<DialogWindowBase<TResult>>(viewType) ?? throw new InvalidOperationException($"Unable to create a view instance for {viewModelName}");
    }

    private DialogViewModelBase<TResult> CreateViewModel<TResult, TViewModel>()
        where TViewModel : IViewModel
        where TResult : DialogResultBase
    {
        var viewModelName = typeof(TViewModel).Name;
        var viewModelType = GetViewModelType(viewModelName) ?? throw new InvalidOperationException($"ViewModel '{viewModelName}' was not found!");
        return GetViewModel<DialogViewModelBase<TResult>>(viewModelType) ?? throw new InvalidOperationException($"Unable to create a ViewModel instance for {viewModelName}");
    }

    private static Type? GetViewModelType(string viewModelName)
    {
        var viewModelsAssembly = Assembly.GetAssembly(typeof(ViewModelBase)) ?? throw new InvalidOperationException("Broken installation!");
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

    private static async Task<TResult> ShowDialogAsync<TResult>(DialogWindowBase<TResult> window)
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

    private static Window? GetMainWindow()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;
        return lifetime?.MainWindow;
    }
    #endregion
}