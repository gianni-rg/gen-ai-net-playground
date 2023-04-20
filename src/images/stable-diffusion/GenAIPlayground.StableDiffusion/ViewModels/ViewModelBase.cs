namespace GenAIPlayground.StableDiffusion.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GenAIPlayground.StableDiffusion.Interfaces.ViewModels;

public abstract class ViewModelBase : ObservableRecipient, IViewModel
{
    public ViewModelBase() : base()
    {
    }

    public ViewModelBase(IMessenger messenger) : base(messenger)
    {
    }

    public object? Parameter { get; set; }

    public virtual void Dispose() { }
}