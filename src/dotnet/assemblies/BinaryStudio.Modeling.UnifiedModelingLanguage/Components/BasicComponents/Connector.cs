namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Connector
        {
        Behavior[] Contract { get; }
        ConnectorEnd[] End { get; }
        ConnectorKind Kind { get; }
        }
    }