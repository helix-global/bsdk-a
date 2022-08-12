namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface EncapsulatedClassifier : StructuredClassifier
        {
        Port[] OwnedPort { get; }
        }
    }