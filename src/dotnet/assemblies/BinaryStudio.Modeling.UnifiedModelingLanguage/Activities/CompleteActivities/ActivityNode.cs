namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityNode
        {
        ActivityGroup[] InGroup { get; }
        InterruptibleActivityRegion[] InInterruptibleRegion { get; }
        }
    }