// Copyright (C) 2023-2024 Gianni Rosa Gallina. All rights reserved.
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

namespace GenAIPlayground.Whisper.DependencyInjection;

using GenAIPlayground.Whisper.Interfaces.Services;
using GenAIPlayground.Whisper.Models.Enums;
using GenAIPlayground.Whisper.Services;
using Splat;
using System;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public static class ServicesBootstrapper
{
    public static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        RegisterCommonServices(services, resolver);
        RegisterPlatformSpecificServices(services, resolver);
    }

    private static void RegisterCommonServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() => new NavigationStore());
        //services.RegisterLazySingleton<ISystemDialogService>(() => new SystemDialogService(resolver.GetRequiredService<IMainWindowProvider>()));
        services.RegisterLazySingleton<INavigationService>(() => new NavigationService(resolver, resolver.GetRequiredService<NavigationStore>()));
        services.RegisterLazySingleton<IDialogService>(() => new DialogService(resolver));
    }

    private static void RegisterPlatformSpecificServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        var platformService = resolver.GetRequiredService<IPlatformService>();
        var platform = platformService.GetPlatform();

        switch (platform)
        {
            case Platform.Linux:
                RegisterLinuxServices(services, resolver);
                break;
            case Platform.MacOs:
                RegisterMacServices(services, resolver);
                break;
            case Platform.Windows:
                RegisterWindowsServices(services, resolver);
                break;
            case Platform.Unknown:
                throw new InvalidOperationException("Unsupported platform");
            default:
                throw new ArgumentOutOfRangeException(nameof(platform));
        }
    }

    private static void RegisterLinuxServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
    }

    private static void RegisterMacServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
    }

    private static void RegisterWindowsServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
    }
}
