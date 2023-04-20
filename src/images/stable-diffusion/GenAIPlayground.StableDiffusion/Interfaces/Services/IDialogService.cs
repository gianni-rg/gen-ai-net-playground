namespace GenAIPlayground.StableDiffusion.Interfaces.Services;

using GenAIPlayground.StableDiffusion.Models.Dialog;
using System.Threading.Tasks;

public interface IDialogService
{
    Task<TResult> ShowDialogAsync<TResult>(string viewModelName)
        where TResult : DialogResultBase;

    Task ShowDialogAsync(string viewModelName);

    Task ShowDialogAsync<TParameter>(string viewModelName, TParameter parameter)
        where TParameter : NavigationParameterBase;

    Task<TResult> ShowDialogAsync<TResult, TParameter>(string viewModelName, TParameter parameter)
        where TResult : DialogResultBase
        where TParameter : NavigationParameterBase;
}