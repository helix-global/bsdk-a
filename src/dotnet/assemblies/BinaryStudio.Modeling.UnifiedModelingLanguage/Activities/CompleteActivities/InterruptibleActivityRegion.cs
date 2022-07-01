namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InterruptibleActivityRegion : ActivityGroup
        {
        ActivityEdge[] InterruptingEdge { get; }
        ActivityNode[] Node { get; }
        }
    }