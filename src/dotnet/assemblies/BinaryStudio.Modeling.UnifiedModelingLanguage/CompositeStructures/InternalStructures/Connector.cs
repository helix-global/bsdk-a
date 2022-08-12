namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Connector : Feature
        {
        //ConnectorEnd[] End { get; }
        Connector[] RedefinedConnector { get; }
        Association Type { get; }
        }
    }