namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityNode : RedefinableElement
        {
        ActivityEdge[] Incoming { get; }
        ActivityEdge[] Outgoing { get; }
        ActivityNode[] RedefinedNode { get; }
        }
    }