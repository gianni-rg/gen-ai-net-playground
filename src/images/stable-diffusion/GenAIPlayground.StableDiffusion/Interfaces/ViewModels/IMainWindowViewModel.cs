namespace GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface IMainWindowViewModel : IViewModel
{
    IMainViewModel MainViewModel { get; }
    bool ShowOverlay { get; set; }
    string WindowTitle { get; set; }
}
