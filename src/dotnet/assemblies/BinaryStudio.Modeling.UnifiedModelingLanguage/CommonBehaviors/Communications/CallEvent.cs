namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CallEvent : MessageEvent
        {
        Operation Operation { get; }
        }
    }