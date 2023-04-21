// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

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