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

namespace GenAIPlayground.LLMs.CLI;

using LLama;
using LLama.Common;
using System;
using System.Globalization;
using System.Reflection;

internal class Program
{
    // Based on LLama.Examples from LLamaSharp: https://github.com/SciSharp/LLamaSharp

    private static readonly string LocalModelsDirectory = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "Models");
    //private static readonly string DefaultModelUri = "https://huggingface.co/TheBloke/CodeLlama-7B-Instruct-GGUF/resolve/main/codellama-7b-instruct.Q4_K_S.gguf";
    private static readonly string DefaultModelUri = "https://huggingface.co/TheBloke/CodeLlama-13B-Instruct-GGUF/resolve/main/codellama-13b-instruct.Q5_K_S.gguf";

    private static readonly string InstructionPrefix = "[INST]";
    private static readonly string InstructionSuffix = "[/INST]";
    private static readonly string SystemInstruction = "You're an intelligent, concise coding assistant. Wrap code in ``` for readability. Don't repeat yourself. Use best practice and good coding standards.";

    public static async Task Main(string[] args)
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        SetEnvForCuda("12.3");

        var modelPath = args.Length > 0 ? args[0] : string.Empty;

        if (string.IsNullOrWhiteSpace(modelPath))
        {
            modelPath = await GetDefaultModel();
        }

        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            GpuLayerCount = 60, // set to the proper value, depending on your available GPU VRAM
        };

        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);

        var executor = new InstructExecutor(context, InstructionPrefix, InstructionSuffix, null);

        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions." +
                        "\nIt's a Code Llama model, so it's trained for programming tasks like \"Write a C# function reading a file name from a given URI\" or \"Write some programming interview questions\"." +
                        "\nWrite 'exit' to exit");

        Console.ForegroundColor = ConsoleColor.White;

        var inferenceParams = new InferenceParams()
        {
            Temperature = 0.8f,
            MaxTokens = -1,
        };

        string instruction = $"{SystemInstruction}\n\n";
        await Console.Out.WriteAsync("Instruction: ");
        instruction += Console.ReadLine() ?? "Ask me for instructions.";
        while (instruction != "exit")
        {

            Console.ForegroundColor = ConsoleColor.Green;
            await foreach (var text in executor.InferAsync(instruction + System.Environment.NewLine, inferenceParams))
            {
                Console.Write(text);
            }
            Console.ForegroundColor = ConsoleColor.White;

            await Console.Out.WriteAsync("Instruction: ");
            instruction = Console.ReadLine() ?? "Ask me for instructions.";
        }
    }

    private static void SetEnvForCuda(string cudaVersion)
    {
        Environment.SetEnvironmentVariable("PATH", $"C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v{cudaVersion}\\bin;C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v{cudaVersion}\\libnvvp;{Environment.GetEnvironmentVariable("PATH")}");
        Environment.SetEnvironmentVariable("CUDA_PATH", $"C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v{cudaVersion}");
    }

    private static async Task<string> GetDefaultModel()
    {
        var uri = new Uri(DefaultModelUri);
        var modelName = uri.Segments[^1];

        Console.WriteLine($"The following model will be used: {modelName}");

        var modelPath = Path.Combine(LocalModelsDirectory, modelName);
        if (!Directory.Exists(LocalModelsDirectory))
        {
            Directory.CreateDirectory(LocalModelsDirectory);
        }

        if (File.Exists(modelPath))
        {
            Console.WriteLine($"Existing model found, using {modelPath}");
        }
        else
        {
            Console.WriteLine($"Model not found locally, downloading {DefaultModelUri}. Please wait...");

            using (var http = new HttpClient())
            {
                using (var downloadStream = await http.GetStreamAsync(uri))
                {
                    using (var fileStream = new FileStream(modelPath, FileMode.Create, FileAccess.Write))
                    {
                        await downloadStream.CopyToAsync(fileStream);
                    }
                }
            }

            Console.WriteLine($"Model downloaded and saved to {modelPath}");
        }


        return modelPath;
    }
}