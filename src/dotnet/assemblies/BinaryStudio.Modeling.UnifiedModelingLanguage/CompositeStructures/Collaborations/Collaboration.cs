namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Collaboration : StructuredClassifier, Classifier, BehavioredClassifier
        {
        ConnectableElement[] CollaborationRole { get; }
        }
    }