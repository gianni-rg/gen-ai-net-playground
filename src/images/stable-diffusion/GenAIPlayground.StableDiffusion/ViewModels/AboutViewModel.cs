// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using System.Reflection;
using System.Runtime.InteropServices;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public partial class AboutViewModel : DialogViewModelBase, IAboutViewModel
{
    #region Private fields
    #endregion

    #region Properties
    [ObservableProperty]
    private string _applicationVersion;
    #endregion

    #region Constructor
    public AboutViewModel()
    {
        ApplicationVersion = $"{Assembly.GetEntryAssembly()?.GetName().Version} ({RuntimeInformation.RuntimeIdentifier})";
    }
    #endregion

    #region Private methods
    #endregion
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
