namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ComponentRealization : Realization
        {
        Component Abstraction { get; }
        Classifier[] RealizingClassifier { get; }
        }
    }