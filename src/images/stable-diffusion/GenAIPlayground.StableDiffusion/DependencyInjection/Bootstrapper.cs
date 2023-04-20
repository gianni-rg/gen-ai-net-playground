namespace GenAIPlayground.StableDiffusion.DependencyInjection;

using Splat;

public static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver, string[] args)
    {
        EnvironmentServicesBootstrapper.RegisterEnvironmentServices(services, resolver);
        ConfigurationBootstrapper.RegisterConfiguration(services, resolver, args);
        LoggingBootstrapper.RegisterLogging(services, resolver);
        AvaloniaServicesBootstrapper.RegisterAvaloniaServices(services);
        ServicesBootstrapper.RegisterServices(services, resolver);
        ViewModelsBootstrapper.RegisterViewModels(services, resolver);
    }
}