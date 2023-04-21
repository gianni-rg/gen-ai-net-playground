# Generative AI Playground - Whisper Build and Publish Script
# Copyright (C) Gianni Rosa Gallina.
# Licensed under the Apache License, Version 2.0.

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 1
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1

# Build Windows executable
dotnet publish .\GenAIPlayground.Whisper\GenAIPlayground.Whisper.csproj -c Release -f net7.0 -r win-x64 --self-contained /p:PublishSingleFile=true

echo "Package available at: .\GenAIPlayground.Whisper\bin\Release\net7.0\win-x64\publish"