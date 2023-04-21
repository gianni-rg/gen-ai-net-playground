// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.ViewModels;

using GenAIPlayground.Whisper.Models.Dialog;
using System.Threading;
using System.Threading.Tasks;

public abstract class ParameterizedDialogViewModelBase<TResult, TParameter> : DialogViewModelBase<TResult>
    where TResult : DialogResultBase
    where TParameter : NavigationParameterBase
{
    public abstract void Activate(TParameter parameter);
}

public abstract class ParameterizedDialogViewModelBaseAsync<TResult, TParameter> : DialogViewModelBase<TResult>
    where TResult : DialogResultBase
    where TParameter : NavigationParameterBase
{
    public abstract Task ActivateAsync(TParameter parameter, CancellationToken cancellationToken = default);
}

public abstract class ParameterizedDialogViewModelBase<TParameter> : ParameterizedDialogViewModelBase<DialogResultBase, TParameter>
    where TParameter : NavigationParameterBase
{

}