// Copyright (C) Gianni Rosa Gallina.
// Licensed under the Apache License, Version 2.0.

namespace Lab.ACG.AnnotationTool.UI.Interfaces.Services;

using Avalonia.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISystemDialogService
{
    Task<string?> GetDirectoryAsync(string? initialDirectory = null);
    Task<string?> SaveFileAsync(string? initialFile = null, string? defaultExtension = null, string? initialDirectory = null, string? dialogTitle = null, List<FileDialogFilter>? filters = null);
    Task<string[]?> OpenFileAsync(string? initialFile = null, bool allowMultiple = false, string? initialDirectory = null, string? dialogTitle = null, List<FileDialogFilter>? filters = null);
}