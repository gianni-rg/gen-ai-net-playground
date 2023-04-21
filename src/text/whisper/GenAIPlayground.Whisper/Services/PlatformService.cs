// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Services;

using GenAIPlayground.Whisper.Interfaces.Services;
using GenAIPlayground.Whisper.Models.Enums;
using System.Runtime.InteropServices;

public class PlatformService : IPlatformService
{
    public Platform GetPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Platform.Linux;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Platform.MacOs;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Platform.Windows;
        }

        return Platform.Unknown;
    }
}