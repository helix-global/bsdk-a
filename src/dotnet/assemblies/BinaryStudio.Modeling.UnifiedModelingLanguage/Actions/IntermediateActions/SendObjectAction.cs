namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface SendObjectAction : InvocationAction
        {
        InputPin Request { get; }
        InputPin Target { get; }
        }
    }