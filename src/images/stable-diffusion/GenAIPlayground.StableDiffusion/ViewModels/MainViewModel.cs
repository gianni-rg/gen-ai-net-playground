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

namespace GenAIPlayground.StableDiffusion.ViewModels;

using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.Models;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public partial class MainViewModel : ViewModelBase,
                                    IMainViewModel,
                                    IRecipient<UpdateStatusBarMessage>,
                                    IRecipient<BusyStatusChangedMessage>,
                                    IRecipient<NotifyErrorMessage>
{
    #region Private fields
    private readonly ILogger _logger;
    private readonly INavigationStore _navigationStore;
    private readonly IDialogService _dialogService;
    private readonly ISystemDialogService _systemDialogService;
    private readonly INavigationService _navigationService;
    private readonly IImageGeneratorService _imageGeneratorService;
    private readonly Stopwatch _stopwatch;
    #endregion

    #region Properties
    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _executionProvider;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private string _negativePrompt;

    [ObservableProperty]
    private ObservableCollection<Bitmap> _generatedImages;

    [ObservableProperty]
    private int _steps;

    [ObservableProperty]
    private int _imagesPerPrompt;

    [ObservableProperty]
    private float _guidance;

    public IViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;
    public IViewModel? CurrentModalViewModel => null; // _modalNavigationStore.CurrentViewModel;
    public bool IsDialogOpen => false; // _modalNavigationStore.CurrentViewModel != null;
    #endregion

    #region Constructor
    public MainViewModel()
    {
        // DESIGN TIME //
        IsBusy = true;
        UpdateStatusBar("Ready");

        Prompt = "example prompt";
        NegativePrompt = string.Empty;
        GeneratedImages = new ObservableCollection<Bitmap>();
        for (int i = 0; i < 4; i++)
        {
            GeneratedImages.Add(GenerateEmptyImage(512, 512));
        }
        Steps = 25;
        ImagesPerPrompt = 4;
        Guidance = 7.5f;
    }

    public MainViewModel(ILogger logger,
                        IDialogService dialogService,
                        ISystemDialogService systemDialogService,
                        INavigationStore navigationStore,
                        INavigationService navigationService,
                        IImageGeneratorService imageGeneratorService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _navigationStore = navigationStore ?? throw new ArgumentNullException(nameof(navigationStore));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _systemDialogService = systemDialogService ?? throw new ArgumentNullException(nameof(systemDialogService));
        _imageGeneratorService = imageGeneratorService ?? throw new ArgumentNullException(nameof(imageGeneratorService));
        _stopwatch = new Stopwatch();

        Prompt = "A sorcer with a wizard hat casting a fire ball, beautiful painting, detailed illustration, digital art, overdetailed art, concept art, full character, character concept, short hair, full body shot, highly saturated colors, fantasy character, detailed illustration, hd, 4k, digital art, overdetailed art, concept art, Dan Mumford, Greg rutkowski, Victo Ngai";
        NegativePrompt = string.Empty;
        GeneratedImages = new ObservableCollection<Bitmap>();
        Steps = 25;
        ImagesPerPrompt = 4;
        Guidance = 7.5f;
    }

    public override void Dispose()
    {
        _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;
        IsActive = false;
    }

    protected override async void OnActivated()
    {
        await Task.Run(() => Messenger.Send(new BusyStatusChangedMessage(true, true)));
        Task.Run(InitializeImageGeneratorAsync);
    }
    #endregion

    #region Commands
    [RelayCommand]
    private Task ShowAboutDialog() => _dialogService.ShowDialogAsync<AboutViewModel>();

    [RelayCommand]
    private void Exit(string isClosing)
    {
        if (bool.Parse(isClosing))
        {
            return;
        }

        Messenger.Send(new ExitApplicationMessage());
    }

    [RelayCommand]
    private async Task Save(Bitmap bitmap)
    {
        if (bitmap is null)
        {
            return;
        }

        var filePath = await _systemDialogService.SaveFileAsync(
           initialFile: $"sd_{DateTime.Now:yyyyMMdd_HHmmss}.jpg",
           dialogTitle: "Save generated image",
           filters: new List<Avalonia.Controls.FileDialogFilter> {
                new Avalonia.Controls.FileDialogFilter { Name = "JPG image files (*.jpg)", Extensions = new List<string> { "jpg" } },
                new Avalonia.Controls.FileDialogFilter { Name = "All files", Extensions = new List<string> { "*" } },}
       );

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        Messenger.Send(new BusyStatusChangedMessage(true, true));

        bitmap.Save(filePath);

        StatusMessage = $"Saved image to '{filePath}'";

        Messenger.Send(new BusyStatusChangedMessage(false));
    }

    [RelayCommand]
    private async Task Generate()
    {
        Messenger.Send(new BusyStatusChangedMessage(true, true));

        StatusMessage = $"Generating {ImagesPerPrompt} image(s). Please wait...";

        if (GeneratedImages.Count > 0)
        {
            foreach (var generatedImage in GeneratedImages)
            {
                generatedImage.Dispose();
            }
            GeneratedImages.Clear();
        }

        List<Bitmap>? images = null;


        await Task.Run(() =>
        {
            _stopwatch.Restart();

            images = _imageGeneratorService.GenerateImages(Prompt, NegativePrompt, steps: Steps, imagesPerPrompt: ImagesPerPrompt, guidance: Guidance, callback: t => { UpdateStatusBar($"Pipeline is running. Step {t + 1}/{Steps}"); });

            _stopwatch.Stop();
        });

        StatusMessage = $"Generated {ImagesPerPrompt} image(s) in {_stopwatch.ElapsedMilliseconds}ms";

        foreach (var generatedImage in images!)
        {
            GeneratedImages.Add(generatedImage);
        }

        Messenger.Send(new BusyStatusChangedMessage(false));
    }
    #endregion

    #region Message Handlers
    public void Receive(UpdateStatusBarMessage m)
    {
        UpdateStatusBar(m.Text);
    }

    public void Receive(BusyStatusChangedMessage m)
    {
        IsBusy = m.IsBusy;
    }

    public async void Receive(NotifyErrorMessage m)
    {
        await _dialogService.ShowDialogAsync<ErrorMessageViewModel, NotifyErrorMessage>(m);
    }
    #endregion

    #region Private methods
    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    private void UpdateStatusBar(string text)
    {
        StatusMessage = text;
    }

    partial void OnExecutionProviderChanging(string value)
    {
        Task.Run( () => Messenger.Send(new BusyStatusChangedMessage(true, true)));
    }

    partial void OnExecutionProviderChanged(string value)
    {
        Task.Run(() => InitializeImageGeneratorAsync() );
    }

    private async Task InitializeImageGeneratorAsync()
    {
        Messenger.Send(new BusyStatusChangedMessage(true, true));

        StatusMessage = $"Initializing Stable Diffusion pipeline. Please wait...";

        var options = new Dictionary<string, string> {
            { "device_id", "0"},
            //{ "gpu_mem_limit",  "15000000000" }, // 15GB
            { "arena_extend_strategy", "kSameAsRequested" },
            };

        var provider = _imageGeneratorService.Config.Provider; // use ExecutionProvider if you want to change the provider in the UI
        
        string onnxProvider = provider switch
        {
            "CUDA" => "CUDAExecutionProvider",
            "CPU" => "CPUExecutionProvider",
            _ => "CUDAExecutionProvider",
        };

        _stopwatch.Restart();

        await _imageGeneratorService.ConfigureImageGeneratorAsync(_imageGeneratorService.Config.ModelId, onnxProvider, _imageGeneratorService.Config.HalfPrecision, options);

        _stopwatch.Stop();

        Messenger.Send(new BusyStatusChangedMessage(false));
        StatusMessage = $"Stable Diffusion pipeline ready ({_stopwatch.ElapsedMilliseconds}ms)";
    }

    private static Bitmap GenerateEmptyImage(int height, int width)
    {
        var emptyImage = new Image<Rgba32>(height, width, Color.White);

        var font = SystemFonts.CreateFont("Arial", 24f);
        string yourText = "No image generated";

        emptyImage.Mutate(x => x.DrawText(yourText, font, Color.Black, new PointF(emptyImage.Width / 2, emptyImage.Height / 2)));
        using (MemoryStream ms = new MemoryStream())
        {
            emptyImage.Save(ms, PngFormat.Instance);
            ms.Position = 0;
            return new Avalonia.Media.Imaging.Bitmap(ms);
        }
    }
    #endregion
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.