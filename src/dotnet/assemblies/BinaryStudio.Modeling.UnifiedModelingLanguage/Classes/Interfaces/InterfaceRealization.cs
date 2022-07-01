namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InterfaceRealization : Realization
        {
        Interface Contract { get; }
        BehavioredClassifier ImplementingClassifier { get; }
        }
    }