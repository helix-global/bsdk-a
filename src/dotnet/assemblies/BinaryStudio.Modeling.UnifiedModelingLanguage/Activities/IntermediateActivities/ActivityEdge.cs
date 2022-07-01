namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityEdge : RedefinableElement
        {
        ValueSpecification Guard { get; }
        //ActivityGroup[] InGroup { get; }
        ActivityPartition[] InPartition { get; }
        }
    }