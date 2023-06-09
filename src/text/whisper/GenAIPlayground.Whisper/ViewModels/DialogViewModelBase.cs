﻿// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.ViewModels;

using CommunityToolkit.Mvvm.Input;
using GenAIPlayground.Whisper.Models.Dialog;
using System;

public partial class DialogViewModelBase<TResult> : ViewModelBase
    where TResult : DialogResultBase
{
    public event EventHandler<DialogResultEventArgs<TResult?>>? CloseRequested;

    protected DialogViewModelBase()
    {
    }

    protected void Close() => Close(default);

    [RelayCommand]
    protected void Close(TResult? result)
    {
        var args = new DialogResultEventArgs<TResult?>(result);

        var localHandler = CloseRequested;
        if (localHandler != null)
        {
            localHandler(this, args);
        }
    }
}

public class DialogViewModelBase : DialogViewModelBase<DialogResultBase>
{

}