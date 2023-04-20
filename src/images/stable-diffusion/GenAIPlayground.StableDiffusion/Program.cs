namespace GenAIPlayground.StableDiffusion;

using Avalonia;
using Avalonia.Controls;
using GenAIPlayground.StableDiffusion.DependencyInjection;
using Microsoft.Extensions.Logging;
using Splat;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

#if RELEASE
        // Add the event handler for handling non-UI thread exceptions
        //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += (sender, e) => HandleUnhandledException("Non-UI", (Exception)e.ExceptionObject);
        TaskScheduler.UnobservedTaskException += (sender, e) => HandleUnhandledException("Task", e.Exception);
#endif

        try
        {
            // Setup dependency injection
            Bootstrapper.Register(Locator.CurrentMutable, Locator.Current, args);

            // Log application version
            var logger = Locator.Current.GetRequiredService<ILogger>();
            logger.LogInformation("Application version: {version} ({runtime})", Assembly.GetEntryAssembly()?.GetName().Version, RuntimeInformation.RuntimeIdentifier);

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
        }
        catch (Exception e)
        {
#if DEBUG
            throw new Exception("UnhandledException", e);
#else
            HandleUnhandledException("Application", e);
#endif
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }

    private static void HandleUnhandledException(string category, Exception ex)
    {
        var logger = Locator.Current.GetRequiredService<ILogger>();

        logger.LogCritical("Unhandled [{category}] error: {ex}", category, ex);

        Environment.Exit(-1);
    }
}