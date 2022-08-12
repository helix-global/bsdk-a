namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityNode : RedefinableElement
        {
        //Activity Activity { get; }
        //ActivityGroup[] InGroup { get; }
        StructuredActivityNode InStructuredNode { get; }
        }
    }