namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface SignalEvent : MessageEvent
        {
        Signal Signal { get; }
        }
    }