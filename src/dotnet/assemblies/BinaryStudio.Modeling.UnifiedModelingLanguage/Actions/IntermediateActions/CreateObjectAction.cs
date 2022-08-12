namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CreateObjectAction : Action
        {
        Classifier Classifier { get; }
        OutputPin Result { get; }
        }
    }