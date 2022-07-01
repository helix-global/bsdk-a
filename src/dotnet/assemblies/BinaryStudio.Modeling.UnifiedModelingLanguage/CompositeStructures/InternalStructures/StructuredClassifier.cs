namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StructuredClassifier : Classifier
        {
        Property[] OwnedAttribute { get; }
        Connector[] OwnedConnector { get; }
        Property[] Part { get; }
        ConnectableElement[] Role { get; }
        }
    }