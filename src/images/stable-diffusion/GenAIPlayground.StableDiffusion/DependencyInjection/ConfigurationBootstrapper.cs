// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.StableDiffusion.DependencyInjection;

using GenAIPlayground.StableDiffusion.Models.Settings;
using Microsoft.Extensions.Configuration;
using Splat;
using System;
using System.IO;

public static class ConfigurationBootstrapper
{
    public static void RegisterConfiguration(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver, string[] args)
    {
        var configuration = BuildConfiguration(args);

        RegisterLoggingConfiguration(services, configuration);
        RegisterAppSettingsConfiguration(services, configuration);
    }

    private static IConfiguration BuildConfiguration(string[] args)
    {
        var configuration = new ConfigurationBuilder();

#if DEBUG
        var env = "development";
#else
        var env = "release";
#endif

        var environmentVariable = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? env;

        // Only add secrets when in development
        var isDevelopment = string.IsNullOrEmpty(environmentVariable) || environmentVariable.ToLower().StartsWith("development");
        if (isDevelopment)
        {
            configuration.AddUserSecrets<Program>();
        }

        // Environment variables can override settings in appsettings.json
        // Remember to configure variables as "GAIP-SD_<section>__<key>" (and restart Visual Studio or Terminal, if open)
        configuration.AddEnvironmentVariables(prefix: "GAIP-SD_");

        // Command-line args can override settings in appsettings.json
        configuration.AddCommandLine(args);

        return configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentVariable}.json", true, false)
            .Build();
    }

    private static void RegisterLoggingConfiguration(IMutableDependencyResolver services, IConfiguration configuration)
    {
        var config = new LoggingSettings();
        configuration.GetSection("Logger").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterAppSettingsConfiguration(IMutableDependencyResolver services, IConfiguration configuration)
    {
        var config = new AppSettings();
        configuration.GetSection("Settings").Bind(config);
        services.RegisterConstant(config);
    }
}