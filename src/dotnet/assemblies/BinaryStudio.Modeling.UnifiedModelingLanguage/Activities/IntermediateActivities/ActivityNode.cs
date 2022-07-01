namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityNode : RedefinableElement
        {
        //ActivityGroup[] InGroup { get; }
        ActivityPartition[] InPartition { get; }
        }
    }