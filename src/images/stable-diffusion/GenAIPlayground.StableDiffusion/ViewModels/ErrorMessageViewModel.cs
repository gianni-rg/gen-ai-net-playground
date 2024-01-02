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

using CommunityToolkit.Mvvm.ComponentModel;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;
using GenAIPlayground.StableDiffusion.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public partial class ErrorMessageViewModel : ParameterizedDialogViewModelBase<NotifyErrorMessage>, IErrorMessageViewModel
{
    #region Private fields
    [ObservableProperty]
    private string _message;
    #endregion

    #region Properties
    #endregion

    #region Constructor
    public ErrorMessageViewModel()
    {
        // DESIGN-TIME
        Message = "Error message";
    }

    public override void Activate(NotifyErrorMessage m)
    {
        Message = m.Message;
    }
    #endregion

    #region Private methods
    #endregion
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
