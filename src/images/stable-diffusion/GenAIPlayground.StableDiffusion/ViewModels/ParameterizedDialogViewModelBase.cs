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

namespace GenAIPlayground.StableDiffusion.ViewModels;

using GenAIPlayground.StableDiffusion.Models.Dialog;
using System.Threading;
using System.Threading.Tasks;

public abstract class ParameterizedDialogViewModelBase<TResult, TParameter> : DialogViewModelBase<TResult>
    where TResult : DialogResultBase
    where TParameter : NavigationParameterBase
{
    public abstract void Activate(TParameter parameter);
}

public abstract class ParameterizedDialogViewModelBaseAsync<TResult, TParameter> : DialogViewModelBase<TResult>
    where TResult : DialogResultBase
    where TParameter : NavigationParameterBase
{
    public abstract Task ActivateAsync(TParameter parameter, CancellationToken cancellationToken = default);
}

public abstract class ParameterizedDialogViewModelBase<TParameter> : ParameterizedDialogViewModelBase<DialogResultBase, TParameter>
    where TParameter : NavigationParameterBase
{

}