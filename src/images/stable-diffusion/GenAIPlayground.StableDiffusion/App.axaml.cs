namespace GenAIPlayground.StableDiffusion;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GenAIPlayground.StableDiffusion.DependencyInjection;
using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.ViewModels;
using GenAIPlayground.StableDiffusion.Views;
using Splat;

public partial class App : Application
{
    public static readonly string AppName = "Stable Diffusion (Generative AI Playground)";
    public static readonly string AppStorageFolderName = ".gaip-sd";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);

            var navigationService = Locator.Current.GetRequiredService<INavigationService>();
            navigationService.NavigateTo<IMainViewModel>();

            DataContext = Locator.Current.GetRequiredService<IMainWindowViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = DataContext,
                WindowState = WindowState.Maximized

            };
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}