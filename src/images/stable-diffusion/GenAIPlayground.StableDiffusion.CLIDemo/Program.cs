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

namespace GenAIPlayground.StableDiffusion.CLIDemo;

using SharpDiffusion;
using System.Diagnostics;
using System.Reflection;

internal class Program
{
    static void Main(string[] args)
    {
        var options = new Dictionary<string, string> {
            { "device_id", "0"},
            //{ "gpu_mem_limit",  "15000000000" }, // 15GB
            { "arena_extend_strategy", "kSameAsRequested" },
            };

        var modelId = "D:\\Personal\\GitHub\\hf-diffusers\\stable_diffusion_onnx-runway";
        var provider = "CUDAExecutionProvider";
        //var provider = "CPUExecutionProvider";

        Stopwatch totalStopwatch = Stopwatch.StartNew();

        Console.WriteLine($"Initializing Stable Diffusion pipeline (v1.5 ONNX on {provider}). Please wait...");

        var sdPipeline = OnnxStableDiffusionPipeline.FromPretrained(modelId, provider: provider, sessionOptions: options);

        var initTimeElapsed = totalStopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Stable Diffusion pipeline initialized ({initTimeElapsed}ms).");
        Console.WriteLine();

        var sdConfig = new StableDiffusionConfig
        {
            NumInferenceSteps = 20,
            GuidanceScale = 7.5,
            NumImagesPerPrompt = 5,
        };

        Stopwatch partialStopwatch = Stopwatch.StartNew();

        var prompts = new List<string> {
            "A sorcer with a wizard hat casting a fire ball, beautiful painting, detailed illustration, digital art, overdetailed art, concept art, full character, character concept, short hair, full body shot, highly saturated colors, fantasy character, detailed illustration, hd, 4k, digital art, overdetailed art, concept art, Dan Mumford, Greg rutkowski, Victo Ngai",
            //"A man thinking about something to do, while in the office, indoor, beautiful painting, detailed illustration, digital art, overdetailed art, concept art, full character, character concept"
        };

        var negativePrompts = new List<string> {
            string.Empty,
            //string.Empty,     
        };

        Console.WriteLine($"Pipeline will generate {sdConfig.NumImagesPerPrompt} images per prompt @ {sdConfig.Height}x{sdConfig.Width}");
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

        sdPipeline.Run(prompts,
                       negativePrompts,
                       sdConfig);

        totalStopwatch.Stop();
        partialStopwatch.Stop();

        Console.WriteLine($"Stable Diffusion pipeline completed.\nInit: {initTimeElapsed}ms Run: {partialStopwatch.ElapsedMilliseconds}ms Tot: {totalStopwatch.ElapsedMilliseconds}ms\n");

        var outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Console.WriteLine($"Generated image can be found in:\n{outputPath}");

        Process.Start("explorer.exe", outputPath);
    }
}