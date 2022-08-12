namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface SendSignalAction : InvocationAction
        {
        Signal Signal { get; }
        InputPin Target { get; }
        }
    }