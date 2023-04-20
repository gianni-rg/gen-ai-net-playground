namespace GenAIPlayground.StableDiffusion.Models.Settings;

public class AppSettings
{
    public int MaxDegreeOfParallelism { get; set; }
    public string ConfigurationFolder { get; set; }
    public string ConfigurationFileName { get; set; }

    public string DataFolder { get; set; }


    public AppSettings()
    {
        MaxDegreeOfParallelism = -1;
        ConfigurationFileName = string.Empty;
        ConfigurationFolder = string.Empty;
        DataFolder = string.Empty;
    }
}
