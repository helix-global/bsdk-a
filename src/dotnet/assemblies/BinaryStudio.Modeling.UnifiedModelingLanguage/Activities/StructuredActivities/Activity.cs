namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Activity : Behavior
        {
        //ActivityGroup[] Group { get; }
        //ActivityNode[] Node { get; }
        StructuredActivityNode[] StructuredNode { get; }
        Variable[] Variable { get; }
        }
    }