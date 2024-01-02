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

using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Models.Settings;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Splat;
using System;
using System.IO;

public static class LoggingBootstrapper
{
    public static void RegisterLogging(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() =>
        {
            var config = resolver.GetRequiredService<LoggingSettings>();
            var logFilePath = GetLogFileName(config, resolver);

            var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Default", GetSerilogLevelFromString(config.DefaultLogLevel))
                .MinimumLevel.Override("Microsoft", GetSerilogLevelFromString(config.MicrosoftLogLevel))
                .WriteTo.Console()
                .WriteTo.File(logFilePath, fileSizeLimitBytes: config.LimitBytes)
                .CreateLogger();

            var factory = new SerilogLoggerFactory(logger);
            return factory.CreateLogger("Default");
        });
    }

    private static LogEventLevel GetSerilogLevelFromString(string logLevel)
    {
        return logLevel switch
        {
            "Debug" => LogEventLevel.Debug,
            "Fatal" => LogEventLevel.Fatal,
            "Verbose" => LogEventLevel.Verbose,
            "Information" => LogEventLevel.Information,
            "Warning" => LogEventLevel.Warning,
            _ => LogEventLevel.Debug
        };
    }

    private static string GetLogFileName(LoggingSettings config, IReadonlyDependencyResolver resolver)
    {
        var currentPlatform = resolver.GetRequiredService<IPlatformService>().GetPlatform();
        var currentLogFolder = config.LogFolder;
        string? rootDir = null;

        if (currentPlatform == Models.Enums.Platform.Windows ||
            currentPlatform == Models.Enums.Platform.MacOs ||
            currentPlatform == Models.Enums.Platform.Linux)
        {
            rootDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            currentLogFolder = Path.Combine(App.AppStorageFolderName, currentLogFolder);
        }

        if (string.IsNullOrWhiteSpace(rootDir))
        {
            rootDir = Directory.GetCurrentDirectory();
        }

        string logDirectory = Path.Combine(rootDir, currentLogFolder);

        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        return Path.Combine(logDirectory, config.LogFileName);
    }
}