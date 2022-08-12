namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Variable : TypedElement, MultiplicityElement
        {
        Activity ActivityScope { get; }
        StructuredActivityNode Scope { get; }
        }
    }