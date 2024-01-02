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

namespace GenAIPlayground.StableDiffusion.Views;

using Avalonia;
using Avalonia.Controls;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closing += MainWindow_Closing;
        Opened += MainWindow_Opened;
        Closed += MainWindow_Closed;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    #region Window Event Handlers
    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var mainWindowViewModel = (IMainWindowViewModel?)DataContext;
        mainWindowViewModel?.MainViewModel.ExitCommand.Execute("true");
    }

    private void MainWindow_Closed(object? sender, System.EventArgs e)
    {
        var mainWindowViewModel = (IMainWindowViewModel?)DataContext;
        if (mainWindowViewModel is null)
        {
            return;
        }

        mainWindowViewModel.IsActive = false;
    }

    private void MainWindow_Opened(object? sender, System.EventArgs e)
    {
        var mainWindowViewModel = (IMainWindowViewModel?)DataContext;
        if (mainWindowViewModel is null)
        {
            return;
        }

        mainWindowViewModel.IsActive = true;
    }
    #endregion
}