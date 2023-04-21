// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace GenAIPlayground.Whisper.Interfaces.Services;

using GenAIPlayground.Whisper.Models.Enums;

public interface IPlatformService
{
    Platform GetPlatform();
}