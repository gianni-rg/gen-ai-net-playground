// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Models.Settings;

public class LoggingSettings
{
    public string LogFileName { get; set; }

    public long LimitBytes { get; set; }

    public string DefaultLogLevel { get; set; }

    public string LogFolder { get; set; }

    public string MicrosoftLogLevel { get; set; }

    public LoggingSettings()
    {
        LogFileName = string.Empty;
        LimitBytes = -1;
        DefaultLogLevel = string.Empty;
        MicrosoftLogLevel = string.Empty;
        LogFolder = string.Empty;
    }
}
