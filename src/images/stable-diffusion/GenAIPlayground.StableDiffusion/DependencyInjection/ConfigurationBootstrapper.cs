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
        RegisterImageGeneratorSettingsConfiguration(services, configuration);
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

    private static void RegisterImageGeneratorSettingsConfiguration(IMutableDependencyResolver services, IConfiguration configuration)
    {
        var config = new ImageGeneratorSettings();
        configuration.GetSection("ImageGenerator").Bind(config);
        services.RegisterConstant(config);
    }
}