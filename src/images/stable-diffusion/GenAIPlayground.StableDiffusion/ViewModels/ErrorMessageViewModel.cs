// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public partial class ErrorMessageViewModel : ParameterizedDialogViewModelBase<NotifyErrorMessage>, IErrorMessageViewModel
{
    #region Private fields
    [ObservableProperty]
    private string _message;
    #endregion

    #region Properties
    #endregion

    #region Constructor
    public ErrorMessageViewModel()
    {
        // DESIGN-TIME
        Message = "Error message";
    }

    public override void Activate(NotifyErrorMessage m)
    {
        Message = m.Message;
    }
    #endregion

    #region Private methods
    #endregion
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
