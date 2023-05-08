// Copyright (C) 2023 Gianni Rosa Gallina. All rights reserved.
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

            var mainWindow = new MainWindow
            {
                DataContext = Locator.Current.GetRequiredService<IMainWindowViewModel>(),
                WindowState = WindowState.Maximized

            };
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}