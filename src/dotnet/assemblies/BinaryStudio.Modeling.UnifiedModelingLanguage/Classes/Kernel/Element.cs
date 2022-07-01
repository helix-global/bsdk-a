namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Element
        {
        Comment[] OwnedComment { get; }
        Element[] OwnedElement { get; }
        Element Owner { get; }
        }
    }