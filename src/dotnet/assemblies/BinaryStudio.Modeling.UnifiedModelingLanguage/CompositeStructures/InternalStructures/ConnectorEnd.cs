namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ConnectorEnd : MultiplicityElement
        {
        Property DefiningEnd { get; }
        //ConnectableElement Role { get; }
        }
    }