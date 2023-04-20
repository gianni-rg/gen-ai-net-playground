namespace GenAIPlayground.StableDiffusion.Interfaces.Services;

using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface INavigationService
{
    bool CanNavigateBack { get; }
    void NavigateTo<TViewModel>(object? parameter = default, bool canNavigateBack = false)
        where TViewModel : IViewModel;
    void NavigateBack(object? parameter = default);
}