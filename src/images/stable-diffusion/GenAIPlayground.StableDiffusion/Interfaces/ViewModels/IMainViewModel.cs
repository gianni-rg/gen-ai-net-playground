namespace GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface IMainViewModel : IViewModel
{
    IViewModel? CurrentViewModel { get; }
    IViewModel? CurrentModalViewModel { get; }
    bool IsDialogOpen { get; }
    string StatusMessage { get; }
    bool IsBusy { get; }
}
