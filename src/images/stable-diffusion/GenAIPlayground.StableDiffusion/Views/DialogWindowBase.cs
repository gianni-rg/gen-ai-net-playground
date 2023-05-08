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

namespace GenAIPlayground.StableDiffusion.Views;

using Avalonia.Controls;
using GenAIPlayground.StableDiffusion.Models.Dialog;
using GenAIPlayground.StableDiffusion.ViewModels;
using System;

public class DialogWindowBase<TResult> : Window
    where TResult : DialogResultBase
{
    private Window? ParentWindow => (Window?)Owner;

    protected DialogViewModelBase<TResult>? ViewModel => DataContext as DialogViewModelBase<TResult>;

    protected DialogWindowBase()
    {
        SubscribeToViewEvents();
    }

    protected virtual void OnOpened()
    {

    }

    private void OnOpened(object? sender, EventArgs e)
    {
        LockSize();
        OnOpened();
    }

    private void LockSize()
    {
        MaxWidth = MinWidth = Width;
        MaxHeight = MinHeight = Height;
    }

    private void SubscribeToViewModelEvents()
    {
        if (ViewModel is not null)
        {
            ViewModel.CloseRequested += ViewModelOnCloseRequested;
        }
    }

    private void UnsubscribeFromViewModelEvents()
    {
        if (ViewModel is not null)
        {
            ViewModel.CloseRequested -= ViewModelOnCloseRequested;
        }
    }

    private void SubscribeToViewEvents()
    {
        DataContextChanged += OnDataContextChanged;
        Opened += OnOpened;
    }

    private void UnsubscribeFromViewEvents()
    {
        DataContextChanged -= OnDataContextChanged;
        Opened -= OnOpened;
    }

    private void OnDataContextChanged(object? sender, EventArgs e) => SubscribeToViewModelEvents();

    private void ViewModelOnCloseRequested(object? sender, DialogResultEventArgs<TResult?> args)
    {
        UnsubscribeFromViewModelEvents();
        UnsubscribeFromViewEvents();

        if (args.Result is not null)
        {
            Close(args.Result);
        }
        else
        {
            Close();
        }
    }
}

public class DialogWindowBase : DialogWindowBase<DialogResultBase>
{

}