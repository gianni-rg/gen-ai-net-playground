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

namespace GenAIPlayground.StableDiffusion.ViewModels;

using CommunityToolkit.Mvvm.Input;
using GenAIPlayground.StableDiffusion.Models.Dialog;
using System;

public partial class DialogViewModelBase<TResult> : ViewModelBase
    where TResult : DialogResultBase
{
    public event EventHandler<DialogResultEventArgs<TResult?>>? CloseRequested;

    protected DialogViewModelBase()
    {
    }

    protected void Close() => Close(default);

    [RelayCommand]
    protected void Close(TResult? result)
    {
        var args = new DialogResultEventArgs<TResult?>(result);

        var localHandler = CloseRequested;
        if (localHandler != null)
        {
            localHandler(this, args);
        }
    }
}

public class DialogViewModelBase : DialogViewModelBase<DialogResultBase>
{

}