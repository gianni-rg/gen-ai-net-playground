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

using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;
using System;

namespace GenAIPlayground.StableDiffusion.CLIDemo.Helpers;

using System.Text.Json;


/// <summary>
/// An HTTP client for the HuggingFace API
/// </summary>
public class HuggingFaceClient
{
    #region Private Fields
    private readonly HttpClient _client;
    private readonly string _baseUri = "https://huggingface.co";
    #endregion

    #region Constructor
    public HuggingFaceClient(HttpClient client)
    {
        _client = client;
    }
    #endregion

    #region Public Methods
    public async Task<List<string>> GetModels(string filter)
    {
        var result = await TryQueryAsync($"models?filter={filter}").ConfigureAwait(false);
        var models = TryParseJson<List<HuggingFaceModelDetails>>(result);
        if (models is null)
        {
            return new List<string>();
        }

        var results = new List<string>(models.Select(m => m.id));

        return results;
    }

    public async Task<HuggingFaceModelDetails?> GetModelAsync(string modelId)
    {
        var result = await TryQueryAsync("models/" + modelId).ConfigureAwait(false);
        return TryParseJson<HuggingFaceModelDetails>(result);
    }

    public async Task<bool> TryDownloadModel(string modelId, HashSet<string> fileset, HashSet<string> optionals, string targetPath, CancellationToken token, Action<int, string> updateStateCallback)
    {
        updateStateCallback(0, "Fetching model metadata...");
        var modelInfo = await GetModelAsync(modelId).ConfigureAwait(false);
        if (modelInfo is null)
        {
            updateStateCallback(1, "Failed to fetch model metadata.");
            return false;
        }

        if (!modelInfo.IsValidModel(fileset))
        {
            updateStateCallback(1, "Model is missing required files.");
            return false;
        }

        updateStateCallback(0, "Ensuring output directory...");
        if (!Directory.Exists(targetPath))
        {
            try
            {
                Directory.CreateDirectory(targetPath);
            }
            catch (Exception ex)
            {
                updateStateCallback(1, "Failed to create output directory.");
                return false;
            }
        }

        try
        {
            var files = new HashSet<string>(fileset);
            files.UnionWith(optionals);

            var fileCount = files.Count;
            var fileIndex = 0;
            foreach (var file in files)
            {
                // Exit loop if cancelled
                if (token.IsCancellationRequested)
                {
                    break;
                }

                // Update state
                updateStateCallback(fileIndex / (fileCount - 1), $"Downloading {file} ({fileIndex + 1}/{fileCount})...");

                // Execute
                var requestResult = await _client.GetAsync($"{_baseUri}/{modelId}/resolve/main/{file}").ConfigureAwait(false);
                if (!requestResult.IsSuccessStatusCode)
                {
                    updateStateCallback(1, $"Failed to download {file}.");
                    return false;
                }

                // Ensure folder
                var targetFilePath = Path.Combine(targetPath, file);
                var targetFolderPath = Path.GetDirectoryName(targetFilePath);
                if (!Directory.Exists(targetFolderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(targetFolderPath);
                    }
                    catch (Exception ex)
                    {
                        updateStateCallback(1, "Failed to create output directory.");
                        return false;
                    }
                }

                // Create file
                var targetStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write);

                // Copy to disk
                var content = requestResult.Content;
                var lengthHeader = content.Headers.TryGetValues("Content-Length", out var lengthValues) ? lengthValues.FirstOrDefault() : null;
                var length = lengthHeader != null ? long.Parse(lengthHeader) : 0;

                var position = 0L;
                var sourceStream = await content.ReadAsStreamAsync().ConfigureAwait(false);

                var buffer = new byte[1024 * 1024];
                while (!token.IsCancellationRequested)
                {
                    var bufferRead = sourceStream.Read(buffer, 0, buffer.Length);

                    position += bufferRead;
                    updateStateCallback(0, $"Downloading {file} ({position / 1024 / 1024}/{length / 1024 / 1024} MB)...");

                    if (bufferRead == 0)
                    {
                        break;
                    }

                    targetStream.Write(buffer, 0, bufferRead);
                }

                sourceStream.Close();
                targetStream.Close();
                fileIndex++;
            }

            if (!token.IsCancellationRequested)
            {
                updateStateCallback(0, "Model downloaded successfully.");
            }
            else
            {
                updateStateCallback(2, "Operation cancelled.");

                foreach (var file in files)
                {
                    var targetFilePath = Path.Combine(targetPath, file);

                    try
                    {
                        File.Delete(targetFilePath);
                    }
                    catch (Exception ex)
                    {
                        // Ignore errors
                    }
                }
            }

            return !token.IsCancellationRequested;
        }
        catch (Exception ex)
        {
            updateStateCallback(1, ex.Message);
            return false;
        }
    }
    #endregion

    #region Private Methods
    private T? TryParseJson<T>(string body)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(body);
        }
        catch (Exception)
        {
            return default;
        }
    }

    private async Task<string> TryQueryAsync(string query)
    {
        try
        {
            var uri = new Uri($"{_baseUri}/api/{query}");
            var response = await _client.GetAsync(uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
    #endregion
}




