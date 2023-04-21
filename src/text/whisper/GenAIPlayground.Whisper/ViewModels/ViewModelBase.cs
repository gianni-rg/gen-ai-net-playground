// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GenAIPlayground.Whisper.Interfaces.ViewModels;

public abstract class ViewModelBase : ObservableRecipient, IViewModel
{
    public ViewModelBase() : base()
    {
    }

    public ViewModelBase(IMessenger messenger) : base(messenger)
    {
    }

    public object? Parameter { get; set; }

    public virtual void Dispose() { }
}