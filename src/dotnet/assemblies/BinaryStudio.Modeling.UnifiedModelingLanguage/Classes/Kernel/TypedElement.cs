namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TypedElement : NamedElement
        {
        Type Type { get; }
        }
    }