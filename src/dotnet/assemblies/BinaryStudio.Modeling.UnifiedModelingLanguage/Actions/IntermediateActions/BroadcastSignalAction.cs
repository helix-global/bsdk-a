namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface BroadcastSignalAction : InvocationAction
        {
        Signal Signal { get; }
        }
    }