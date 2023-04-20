namespace GenAIPlayground.StableDiffusion.DependencyInjection;

using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Models.Enums;
using GenAIPlayground.StableDiffusion.Services;
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
