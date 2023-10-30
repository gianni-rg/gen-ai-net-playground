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

namespace GenAIPlayground.Whisper.CLIDemo;

using global::Whisper.net;
using global::Whisper.net.Ggml;
using System.Globalization;

internal class Program
{
    static async Task Main(string[] args)
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        if (args.Length < 3)
        {
            Console.WriteLine("Usage: whisper-cli <file: path> <language: auto, it, en> <model: tiny, base, small, medium, large>");
            Console.WriteLine("Example: whisper-cli PATH-TO-AUDIO-FILE.wav en large");
            return;
        }

        var fileToProcess = args[0];
        var language = args[1];
        var model = args[2];

        if (!Enum.TryParse<GgmlType>(model, true, out var modelType))
        {
            Console.WriteLine($"Invalid model type '{model}'. Supported models: tiny, base, small, medium, large");
            return;
        }

        var modelName = $"ggml-{modelType.ToString().ToLower()}.bin";

        if (!File.Exists(modelName))
        {
            Console.WriteLine($"Downloading Model '{modelName}'");
            using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(modelType);
            using var fileWriter = File.OpenWrite(modelName);
            await modelStream.CopyToAsync(fileWriter);
        }

        // Same factory can be used by multiple task to create processors.
        using var factory = WhisperFactory.FromPath(modelName);

        var builder = factory.CreateBuilder()
            .WithLanguage(language)
            .WithProbabilities();

        using var processor = builder.Build();

        using var fileStream = File.OpenRead(fileToProcess);

        using var outTranscriptionStream = new StreamWriter(Path.Combine(Path.GetDirectoryName(fileToProcess), $"{Path.GetFileNameWithoutExtension(fileToProcess)}_Transcription_whisper_ggml_{modelType.ToString().ToLower()}.txt"));

        using var outSRTStream = new StreamWriter(Path.Combine(Path.GetDirectoryName(fileToProcess), $"{Path.GetFileNameWithoutExtension(fileToProcess)}_TranscriptionSRT_whisper_ggml_{modelType.ToString().ToLower()}.srt"));

        var segmentIndex = 1;
        var start = DateTime.UtcNow;
        await foreach (var segment in processor.ProcessAsync(fileStream, CancellationToken.None))
        {
            Console.WriteLine($"{segment.Start:hh\\:mm\\:ss\\.ffffff} ==> {segment.End:hh\\:mm\\:ss\\.ffffff} : {segment.Text} ({segment.Probability})");
            outTranscriptionStream.WriteLine($"{segment.Start:hh\\:mm\\:ss\\.ffffff} ==> {segment.End:hh\\:mm\\:ss\\.ffffff} : {segment.Text} ({segment.Probability})");

            // Write segmentText in SRT format
            //"1 00:00:00:09 --> 00:00:21:25 TEXT\n";
            outSRTStream.Write($"{segmentIndex}\r\n{GetTimecodeSRT(segment.Start)} --> {GetTimecodeSRT(segment.End)}\r\n{segment.Text.Trim()}\r\n\r\n");
            segmentIndex++;
        }
        var end = DateTime.UtcNow;
        Console.WriteLine($"Total time: {end - start}");
    }

    // Get the timecode in SRT format
    private static string GetTimecodeSRT(TimeSpan timeSpan)
    {
        return $"{timeSpan:hh\\:mm\\:ss\\,fff}";
    }
}
