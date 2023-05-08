namespace GenAIPlayground.Whisper.CLIDemo;

using global::Whisper.net;
using global::Whisper.net.Ggml;

internal class Program
{
    static async Task Main(string[] args)
    {
        var language = "it";

        //var fileToProcess = @"..\..\..\assets\whisperdemo_EN.wav";
        var fileToProcess = @"..\..\..\assets\whisperdemo_IT.wav";

        var modelType = GgmlType.Small;

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

        await foreach (var segment in processor.ProcessAsync(fileStream, CancellationToken.None))
        {
            Console.WriteLine($"New Segment: {segment.Start} ==> {segment.End} : {segment.Text} ({segment.Probability})");
        }
    }
}
