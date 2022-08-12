namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Relationship : Element
        {
        Element[] RelatedElement { get; }
        }
    }