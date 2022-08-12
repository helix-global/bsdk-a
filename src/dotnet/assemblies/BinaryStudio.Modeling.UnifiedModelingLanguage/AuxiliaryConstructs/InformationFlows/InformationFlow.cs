namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InformationFlow : DirectedRelationship, PackageableElement
        {
        Classifier[] Conveyed { get; }
        NamedElement[] InformationSource { get; }
        NamedElement[] InformationTarget { get; }
        Relationship[] Realization { get; }
        ActivityEdge[] RealizingActivityEdge { get; }
        Connector[] RealizingConnector { get; }
        Message[] RealizingMessage { get; }
        }
    }