namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface BehavioredClassifier : Classifier
        {
        Behavior ClassifierBehavior { get; }
        Behavior[] OwnedBehavior { get; }
        }
    }