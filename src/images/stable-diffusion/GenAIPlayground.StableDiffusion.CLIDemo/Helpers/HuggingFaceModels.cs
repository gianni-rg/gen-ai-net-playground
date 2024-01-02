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

namespace GenAIPlayground.StableDiffusion.CLIDemo.Helpers;

public class HuggingFaceModelDetails
{
    public static readonly HashSet<string> StableDiffusionOnnxFileset = new HashSet<string> {
        "feature_extractor/preprocessor_config.json",
        "safety_checker/model.onnx",
        "scheduler/scheduler_config.json",
        "text_encoder/model.onnx",
        "tokenizer/merges.txt",
        "tokenizer/special_tokens_map.json",
        "tokenizer/tokenizer_config.json",
        "tokenizer/vocab.json",
        "unet/model.onnx",
        "vae_decoder/model.onnx",
        "vae_encoder/model.onnx"
     };

    public static readonly HashSet<string> StableDiffusionOnnxOptionals = new HashSet<string> {
        "controlnet/model.onnx"
     };

    public string _id { get; set; }
    public string id { get; set; }
    public string modelId { get; set; }
    public string author { get; set; }
    public string sha { get; set; }
    public DateTime lastModified { get; set; }
    public bool _private { get; set; }
    public bool disabled { get; set; }
    public bool gated { get; set; }
    public string pipeline_tag { get; set; }
    public string[] tags { get; set; }
    public int downloads { get; set; }
    public string library_name { get; set; }
    public int likes { get; set; }
    public object modelindex { get; set; }
    public HuggingFaceModelDetailsConfig config { get; set; }
    public HuggingFaceModelDetailsCardData cardData { get; set; }
    public object[] spaces { get; set; }
    public HuggingFaceModelDetailsSibling[] siblings { get; set; }

    public bool IsValidModel(HashSet<string> fileset)
    {
        var count = 0;
        foreach (var file in siblings)
        {
            if (fileset.Contains(file.rfilename))
            {
                count++;
            }
        }

        return count == fileset.Count;
    }
}

public class HuggingFaceModelDetailsConfig
{
    public HuggingFaceModelDetailsDiffusers diffusers { get; set; }
}

public class HuggingFaceModelDetailsDiffusers
{
    public string class_name { get; set; }
}

public class HuggingFaceModelDetailsCardData
{
    public string pipeline_tag { get; set; }
    public string[] tags { get; set; }
    public string library_name { get; set; }
    public HuggingFaceModelDetailsCardDataModelDescription[] model_description { get; set; }
}

public class HuggingFaceModelDetailsCardDataModelDescription
{
    public string repo { get; set; }
}

public class HuggingFaceModelDetailsSibling
{
    public string rfilename { get; set; }
}
