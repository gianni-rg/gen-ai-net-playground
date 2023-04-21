// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.DependencyInjection;

using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.Models.Enums;
using GenAIPlayground.StableDiffusion.Services;
using GenAIPlayground.StableDiffusion.ViewModels;
using Splat;
using System;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public static class ViewModelsBootstrapper
{
    public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        RegisterCommonViewModels(services, resolver);
        RegisterPlatformSpecificViewModels(services, resolver);
    }

    private static void RegisterCommonViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton<IMainWindowViewModel>(() => new MainWindowViewModel(
            resolver.GetRequiredService<IMainViewModel>(),
            resolver.GetRequiredService<ILogger>()
        ));

        services.RegisterLazySingleton<IMainViewModel>(() => new MainViewModel(
            resolver.GetRequiredService<ILogger>(),
            resolver.GetRequiredService<IDialogService>(),
            resolver.GetRequiredService<NavigationStore>(),
            resolver.GetRequiredService<INavigationService>()
        ));

        services.Register<IAboutViewModel>(() => new AboutViewModel());
    }

    private static void RegisterPlatformSpecificViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        var platformService = resolver.GetRequiredService<IPlatformService>();
        var platform = platformService.GetPlatform();

        switch (platform)
        {
            case Platform.Linux:
                RegisterLinuxViewModels(services, resolver);
                break;
            case Platform.MacOs:
                RegisterMacViewModels(services, resolver);
                break;
            case Platform.Windows:
                RegisterWindowsViewModels(services, resolver);
                break;
            case Platform.Unknown:
                throw new InvalidOperationException("Unsupported platform");
            default:
                throw new ArgumentOutOfRangeException(nameof(platform));
        }
    }

    private static void RegisterLinuxViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
    }

    private static void RegisterMacViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
    }

    private static void RegisterWindowsViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
    }
}
