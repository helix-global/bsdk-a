namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface RaiseExceptionAction : Action
        {
        InputPin Exception { get; }
        }
    }