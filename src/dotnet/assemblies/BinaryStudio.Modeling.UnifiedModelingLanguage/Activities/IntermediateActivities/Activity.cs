namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Activity
        {
        //ActivityGroup[] Group { get; }
        ActivityPartition[] Partition { get; }
        }
    }