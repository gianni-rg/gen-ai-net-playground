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

namespace GenAIPlayground.Whisper.AzureEndpoint;

using System.Net.Http.Headers;
using System.Text.Json;

internal class Program
{
    // Ported from https://github.com/Azure/azureml-examples/blob/main/sdk/python/foundation-models/system/inference/automatic-speech-recognition/asr-online-endpoint.ipynb
    static async Task Main(string[] args)
    {
        Console.WriteLine("Whisper Endpoint Demo\n=====================");

        var endpointInstanceName = "YOUR-DEPLOYED-INSTANCE-NAME";
        var endpointLocation = "YOUR-DEPLOYED-INSTANCE-LOCATION";
        var endpointUri = new Uri($"https://{endpointInstanceName}.{endpointLocation}.inference.ml.azure.com/score");

        var audioSourceUri = "URL-TO-YOUR-FILE-TO-TRANSCRIBE.wav";
        var audioSourceLanguage = "en";

        var requestPayload = new WhisperEndpointRequest
        {
            inputs = new Inputs
            {
                audio = new string[] { audioSourceUri },
                language = new string[] { audioSourceLanguage },
            }
        };

        var handler = new HttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
        };

        using (var client = new HttpClient(handler))
        {
            string apiKey = File.ReadAllText("key.txt");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("A key should be provided to invoke the endpoint");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.BaseAddress = endpointUri;

            var content = new StringContent(JsonSerializer.Serialize(requestPayload));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // This header will force the request to go to a specific deployment.
            // Remove this line to have the request observe the endpoint traffic rules
            content.Headers.Add("azureml-model-deployment", "openai-whisper-large-demo");

            Console.WriteLine($"Transcribing '{audioSourceUri}'");
            Console.WriteLine($"Sending request. Please wait...");
            var response = await client.PostAsync(string.Empty, content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Result: {result}");
            }
            else
            {
                Console.WriteLine($"The request failed with status code: {response.StatusCode}");
                Console.WriteLine(response.Headers.ToString());

                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
            }
            Console.ReadLine();
        }
    }
}