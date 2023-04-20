namespace GenAIPlayground.StableDiffusion.ViewModels;

using System;

public class DialogResultEventArgs<TResult> : EventArgs
{
    public TResult Result { get; }

    public DialogResultEventArgs(TResult result)
    {
        Result = result;
    }
}