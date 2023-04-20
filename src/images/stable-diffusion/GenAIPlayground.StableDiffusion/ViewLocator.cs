using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GenAIPlayground.StableDiffusion.ViewModels;
using System;

namespace GenAIPlayground.StableDiffusion
{
    public class ViewLocator : IDataTemplate
    {
        public IControl? Build(object? data)
        {
            if(data is null)
            {
                return new TextBlock { Text = "View not specified" };
            }

            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type is not null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = $"Not Found: {name}" };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}