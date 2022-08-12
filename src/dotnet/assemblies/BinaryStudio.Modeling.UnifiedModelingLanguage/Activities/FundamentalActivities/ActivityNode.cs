namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityNode : NamedElement
        {
        Activity Activity { get; }
        //ActivityGroup[] InGroup { get; }
        }
    }