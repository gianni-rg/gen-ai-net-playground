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

namespace GenAIPlayground.StableDiffusion.CLIDemo;

using SharpDiffusion;
using SixLabors.ImageSharp;
using System.Diagnostics;

internal class Program
{
    static async Task Main(string[] args)
    {
        var options = new Dictionary<string, string> {
            { "device_id", "0"},
            //{ "gpu_mem_limit",  "15000000000" }, // 15GB
            { "arena_extend_strategy", "kSameAsRequested" },
        };

        var provider = "CUDAExecutionProvider";
        //var provider = "CPUExecutionProvider";

        SetEnvForCuda("11.8");

        var modelId = "PATH_TO_STABLE_DIFFUSION_MODEL_ONNX";
        var halfPrecision = false;

        Stopwatch totalStopwatch = Stopwatch.StartNew();

        Console.WriteLine($"Initializing Stable Diffusion pipeline (v1.5 ONNX on '{provider}' FP16: {halfPrecision}). Please wait...");
        var diffPipeline = DiffusionPipelineFactory.FromPretrained<OnnxStableDiffusionPipeline>(modelId, provider, halfPrecision, options);

        if (diffPipeline is null)
        {
            Console.WriteLine($"ERROR: unable to initialize the pipeline. Check the configuration.");
            return;
        }

        var initTimeElapsed = totalStopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Stable Diffusion pipeline initialized ({initTimeElapsed}ms).");
        Console.WriteLine();

        var sdConfig = new StableDiffusionConfig
        {
            NumInferenceSteps = 25, // Can be set to 1~50 steps. LCM support fast inference even <= 4 steps. Recommend: 1~8 steps.
            GuidanceScale = 7.5,
            NumImagesPerPrompt = 4,
            Seed = null, // Set a specific seed or null to generate a random one

        };

        Stopwatch partialStopwatch = Stopwatch.StartNew();

        var prompts = new List<string> {
            "A sorcer with a wizard hat casting a fire ball, beautiful painting, detailed illustration, digital art, overdetailed art, concept art, full character, character concept, short hair, full body shot, highly saturated colors, fantasy character, detailed illustration, hd, 4k, digital art, overdetailed art, concept art, Dan Mumford, Greg rutkowski, Victo Ngai"
            //"A man thinking about something to do, while in the office, indoor, beautiful painting, detailed illustration, digital art, overdetailed art, concept art, full character, character concept",
            //"Self-portrait oil painting, a beautiful cyborg with golden hair, 8k"
        };

        var negativePrompts = new List<string> {
            string.Empty,
        };

        Console.WriteLine($"Pipeline will generate {sdConfig.NumImagesPerPrompt} image(s) per prompt @ {sdConfig.Height}x{sdConfig.Width}");
        Console.WriteLine();

        Console.WriteLine("Prompts:");
        foreach (var p in prompts)
        {
            Console.WriteLine($"- {p}");
        };

        if (negativePrompts.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Negative prompts:");
            foreach (var p in negativePrompts)
            {
                Console.WriteLine($"- {p}");
            };
        }

        Console.WriteLine();

        Console.WriteLine("Pipeline is running. Please wait...");

        var output = diffPipeline.Run(prompts, negativePrompts, sdConfig, t => { Console.WriteLine($"Pipeline is running. Step {t + 1}/{sdConfig.NumInferenceSteps}"); });

        totalStopwatch.Stop();
        partialStopwatch.Stop();

        Console.WriteLine($"Pipeline completed.\nInit: {initTimeElapsed}ms Run: {partialStopwatch.ElapsedMilliseconds}ms Tot: {totalStopwatch.ElapsedMilliseconds}ms\n");

        if (output.Images is null || output.Images.Count == 0)
        {
            Console.WriteLine($"No images generated");
            return;
        }

        var outputPath = Directory.GetCurrentDirectory();
        Console.WriteLine($"Generated image can be found in:\n{outputPath}");

        for (int i = 0; i < output.Images.Count; i++)
        {
            var nsfw = output.NSFWContentDetected[i] ? "_NSFW" : string.Empty;
            var imageName = $"sd_image_{DateTime.Now:yyyyMMddHHmm}_{i}{nsfw}.png";
            var imagePath = Path.Combine(outputPath, imageName);
            output.Images[i].Save(imagePath);
        }

        Process.Start("explorer.exe", outputPath);
    }

    private static void SetEnvForCuda(string cudaVersion)
    {
        Environment.SetEnvironmentVariable("PATH", $"C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v{cudaVersion}\\bin;C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v{cudaVersion}\\libnvvp;{Environment.GetEnvironmentVariable("PATH")}");
        Environment.SetEnvironmentVariable("CUDA_PATH", $"C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v{cudaVersion}");
    }
}