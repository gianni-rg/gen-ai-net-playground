// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GenAIPlayground.Whisper.DependencyInjection;
using GenAIPlayground.Whisper.Interfaces.Services;
using GenAIPlayground.Whisper.Interfaces.ViewModels;
using GenAIPlayground.Whisper.Views;
using Splat;

public partial class App : Application
{
    public static readonly string AppName = "Whisper (Generative AI Playground)";
    public static readonly string AppStorageFolderName = ".gaip-whisper";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);

            var navigationService = Locator.Current.GetRequiredService<INavigationService>();
            navigationService.NavigateTo<IMainViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = Locator.Current.GetRequiredService<IMainWindowViewModel>(),
                WindowState = WindowState.Maximized

            };
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}