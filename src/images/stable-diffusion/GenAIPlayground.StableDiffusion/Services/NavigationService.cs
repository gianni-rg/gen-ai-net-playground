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

using GenAIPlayground.StableDiffusion.DependencyInjection;
using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using Splat;
using System;
using System.Collections.Generic;

public class NavigationService : INavigationService
{
    private readonly IReadonlyDependencyResolver _resolver;
    private readonly INavigationStore _navigationStore;
    private readonly Stack<IViewModel> _navigationHistory;

    public bool CanNavigateBack => _navigationHistory.Count > 0;

    public NavigationService(IReadonlyDependencyResolver resolver, INavigationStore navigationStore /*, INavigationStore modalNavigationStore*/)
    {
        _resolver = resolver;
        _navigationStore = navigationStore;
        _navigationHistory = new Stack<IViewModel>();
    }

    public void NavigateTo<TViewModel>(object? parameter = default, bool canNavigateBack = false)
        where TViewModel : IViewModel
    {
        if (_navigationStore.CurrentViewModel is not null)
        {
            _navigationStore.CurrentViewModel.IsActive = false;
            if (canNavigateBack)
            {
                _navigationHistory.Push(_navigationStore.CurrentViewModel);
            }
            else
            {
                _navigationStore.CurrentViewModel.Dispose();
            }
        }

        IViewModel viewModel = CreateViewModel<TViewModel>();
        viewModel.Parameter = parameter;

        _navigationStore.CurrentViewModel = viewModel;
        _navigationStore.CurrentViewModel.IsActive = true;
    }

    public void NavigateBack(object? parameter = default)
    {
        if (!CanNavigateBack)
        {
            return;
        }

        if (_navigationStore.CurrentViewModel is not null)
        {
            _navigationStore.CurrentViewModel.IsActive = false;
            _navigationStore.CurrentViewModel.Dispose();
        }

        IViewModel viewModel = _navigationHistory.Pop();
        viewModel.Parameter = parameter;

        _navigationStore.CurrentViewModel = viewModel;
        _navigationStore.CurrentViewModel.IsActive = true;
    }

    private TViewModel CreateViewModel<TViewModel>()
    {
        return _resolver.GetRequiredService<TViewModel>() ?? throw new InvalidOperationException($"Unable to create a ViewModel instance for {typeof(TViewModel)}");
    }
}