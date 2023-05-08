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

        services.Register<IErrorMessageViewModel>(() => new ErrorMessageViewModel());
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
