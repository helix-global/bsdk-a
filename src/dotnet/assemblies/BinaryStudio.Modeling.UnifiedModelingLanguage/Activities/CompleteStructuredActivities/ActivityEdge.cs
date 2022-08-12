namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityEdge : RedefinableElement
        {
        //ActivityGroup[] InGroup { get; }
        StructuredActivityNode InStructuredNode { get; }
        }
    }