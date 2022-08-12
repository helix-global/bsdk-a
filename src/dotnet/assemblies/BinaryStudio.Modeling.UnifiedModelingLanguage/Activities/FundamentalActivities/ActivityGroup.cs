namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ActivityGroup : NamedElement
        {
        //ActivityNode[] ContainedNode { get; }
        Activity InActivity { get; }
        ActivityGroup[] Subgroup { get; }
        ActivityGroup SuperGroup { get; }
        }
    }