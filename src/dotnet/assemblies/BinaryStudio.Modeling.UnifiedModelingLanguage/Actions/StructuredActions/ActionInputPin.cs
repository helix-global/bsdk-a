namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ActionInputPin : InputPin
        {
        Action FromAction { get; }
        }
    }