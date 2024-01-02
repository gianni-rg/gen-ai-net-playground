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

namespace GenAIPlayground.StableDiffusion.Interfaces.Services;

using Avalonia.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISystemDialogService
{
    Task<string?> GetDirectoryAsync(string? initialDirectory = null);
    Task<string?> SaveFileAsync(string? initialFile = null, string? defaultExtension = null, string? initialDirectory = null, string? dialogTitle = null, List<FileDialogFilter>? filters = null);
    Task<string[]?> OpenFileAsync(string? initialFile = null, bool allowMultiple = false, string? initialDirectory = null, string? dialogTitle = null, List<FileDialogFilter>? filters = null);
}