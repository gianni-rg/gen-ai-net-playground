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

        if (currentPlatform == Models.Enums.Platform.MacOs || currentPlatform == Models.Enums.Platform.Linux)
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