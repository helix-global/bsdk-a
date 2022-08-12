namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface BehavioredClassifier : NamedElement, Classifier
        {
        InterfaceRealization[] InterfaceRealization { get; }
        }
    }