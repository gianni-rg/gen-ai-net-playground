namespace GenAIPlayground.StableDiffusion.DependencyInjection;

using Splat;
using System;

public static class ReadonlyDependencyResolverExtensions
{
    public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
    {
        var service = resolver.GetService<TService>();
        return service ?? throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");
    }

    public static object GetRequiredService(this IReadonlyDependencyResolver resolver, Type type)
    {
        var service = resolver.GetService(type);
        return service ?? throw new InvalidOperationException($"Failed to resolve object of type {type}");
    }
}