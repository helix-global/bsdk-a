namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface UseCase : BehavioredClassifier
        {
        Extend[] Extend { get; }
        ExtensionPoint[] ExtensionPoint { get; }
        Include[] Include { get; }
        Classifier[] Subject { get; }
        }
    }