namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReadExtentAction : Action
        {
        Classifier Classifier { get; }
        OutputPin Result { get; }
        }
    }