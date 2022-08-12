namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DataType : Classifier
        {
        Property[] OwnedAttribute { get; }
        Operation[] OwnedOperation { get; }
        }
    }