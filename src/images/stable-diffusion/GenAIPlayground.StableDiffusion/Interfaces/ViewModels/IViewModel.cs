namespace GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public interface IViewModel
{
    object? Parameter { get; set; }
    bool IsActive { get; set; }
    void Dispose();
}
