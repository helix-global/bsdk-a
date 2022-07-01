namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Interface : Classifier
        {
        Classifier[] NestedClassifier { get; }
        Property[] OwnedAttribute { get; }
        Operation[] OwnedOperation { get; }
        Interface[] RedefinedInterface { get; }
        }
    }