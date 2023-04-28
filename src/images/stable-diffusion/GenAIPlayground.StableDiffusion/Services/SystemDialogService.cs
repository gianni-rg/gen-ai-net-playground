// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.Interfaces.Services;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using Lab.ACG.AnnotationTool.UI.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SystemDialogService : ISystemDialogService
{
    #region Constructor
    public SystemDialogService()
    {
    }
    #endregion

    #region Public methods
    public async Task<string?> GetDirectoryAsync(string? initialDirectory = null)
    {
        var dialog = new OpenFolderDialog { Directory = initialDirectory };

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

        mainWindowDataContext.ShowOverlay = true;
        var res = await dialog.ShowAsync(mainWindow);
        mainWindowDataContext.ShowOverlay = false;
        return res;
    }

    public async Task<string?> SaveFileAsync(string? initialFile = null, string? defaultExtension = null, string? initialDirectory = null, string? dialogTitle = null, List<FileDialogFilter>? filters = null)
    {
        var dialog = new SaveFileDialog { InitialFileName = initialFile, DefaultExtension = defaultExtension, Directory = initialDirectory, Title = dialogTitle, Filters = filters };

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

        mainWindowDataContext.ShowOverlay = true;
        var res = await dialog.ShowAsync(mainWindow);
        mainWindowDataContext.ShowOverlay = false;
        return res;
    }

    public async Task<string[]?> OpenFileAsync(string? initialFile = null, bool allowMultiple = false, string? initialDirectory = null, string? dialogTitle = null, List<FileDialogFilter>? filters = null)
    {
        var dialog = new OpenFileDialog { InitialFileName = initialFile, AllowMultiple = allowMultiple, Directory = initialDirectory, Title = dialogTitle, Filters = filters };

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

        mainWindowDataContext.ShowOverlay = true;
        var res = await dialog.ShowAsync(mainWindow);
        mainWindowDataContext.ShowOverlay = false;
        return res;
    }
    #endregion

    #region Private methods
    private Window? GetMainWindow()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime;

        return lifetime?.MainWindow;
    }
    #endregion
}