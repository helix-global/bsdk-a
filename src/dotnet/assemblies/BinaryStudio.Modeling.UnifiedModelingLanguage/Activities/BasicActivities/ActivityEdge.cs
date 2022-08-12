namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityEdge : RedefinableElement
        {
        Activity Activity { get; }
        ActivityGroup[] InGroup { get; }
        ActivityEdge[] RedefinedEdge { get; }
        ActivityNode Source { get; }
        ActivityNode Target { get; }
        }
    }