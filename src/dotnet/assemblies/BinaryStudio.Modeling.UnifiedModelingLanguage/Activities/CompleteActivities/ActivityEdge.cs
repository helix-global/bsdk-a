namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityEdge : RedefinableElement
        {
        InterruptibleActivityRegion Interrupts { get; }
        ValueSpecification Weight { get; }
        }
    }