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

namespace GenAIPlayground.StableDiffusion.Services;

using Avalonia.Media.Imaging;
using GenAIPlayground.StableDiffusion.Interfaces.Services;
using GenAIPlayground.StableDiffusion.Models.Settings;
using MathNet.Numerics.Random;
using Microsoft.Extensions.Logging;
using SharpDiffusion;
using SharpDiffusion.Interfaces;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ImageGeneratorService : IImageGeneratorService
{
    private readonly ILogger _logger;
    private IDiffusionPipeline? _sdPipeline;

    public ImageGeneratorSettings Config { get; }

    public ImageGeneratorService(ILogger logger, ImageGeneratorSettings config)
    {
        _logger = logger;
        Config = config;
    }

    public async Task ConfigureImageGeneratorAsync(string modelId, string onnxProvider, bool halfPrecision, Dictionary<string, string> options)
    {
        _sdPipeline?.Dispose();

        _sdPipeline = DiffusionPipelineFactory.FromPretrained<OnnxStableDiffusionPipeline>(modelId, onnxProvider, halfPrecision, options);

        if (_sdPipeline is null)
        {
            throw new InvalidOperationException("Unable to load the OnnxStableDiffusionPipeline");
        }

        // Dummy pipeline execution (to pre-load ONNX engine)
        var sdConfig = new StableDiffusionConfig
        {
            NumInferenceSteps = 1,
            GuidanceScale = 7,
            NumImagesPerPrompt = 1,
        };
        await Task.Run(() => _sdPipeline.Run(new List<string> { string.Empty }, new List<string> { string.Empty }, sdConfig));

    }
    public List<Bitmap> GenerateImages(string prompt, string negativePrompt, int steps = 20, float guidance = 7.5f, int imagesPerPrompt = 1, int? seed = null, bool safetyCheckEnabled = true, Action<int>? callback = null)
    {
        var bitmaps = new List<Bitmap>();

        _logger.LogInformation("Generating image (steps: {steps}, guidance: {guidance}, imagesPerPrompt: {imagesPerPrompt} Seed: {seed} SafetyCheck: {safetyCheckEnabled})", steps, guidance, imagesPerPrompt, seed, safetyCheckEnabled);

        var sdConfig = new StableDiffusionConfig
        {
            NumInferenceSteps = steps,
            GuidanceScale = guidance,
            NumImagesPerPrompt = imagesPerPrompt,
            Seed = seed
        };

        if (string.IsNullOrWhiteSpace(prompt))
        {
            var emptyImage = GenerateEmptyImage(sdConfig.Height, sdConfig.Width);
            for (int i = 0; i < imagesPerPrompt; i++)
            {
                bitmaps.Add(emptyImage);
            }
            return bitmaps;
        }

        if (string.IsNullOrWhiteSpace(negativePrompt))
        {
            negativePrompt = string.Empty;
        }

        var pipelineOutput = _sdPipeline!.Run(new List<string> { prompt }, new List<string> { negativePrompt }, sdConfig, callback);

        if (pipelineOutput.Images is null || pipelineOutput.Images.Count == 0)
        {
            var emptyImage = GenerateEmptyImage(sdConfig.Height, sdConfig.Width);
            for (int i = 0; i < imagesPerPrompt; i++)
            {
                bitmaps.Add(emptyImage);
            }
            return bitmaps;
        }

        for (int i = 0; i < pipelineOutput.Images.Count; i++)
        {
            if(safetyCheckEnabled && pipelineOutput.NSFWContentDetected[i])
            {
                _logger.LogWarning("NSFW content detected in image #{i:02}", i);
                continue;
            }

            var image = pipelineOutput.Images[i];

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, PngFormat.Instance);
                ms.Position = 0;
                bitmaps.Add(new Bitmap(ms));
            }
        }
        return bitmaps;
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
}
