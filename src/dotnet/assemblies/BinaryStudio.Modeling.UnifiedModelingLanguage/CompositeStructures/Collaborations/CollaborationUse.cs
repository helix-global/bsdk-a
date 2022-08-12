namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CollaborationUse : NamedElement
        {
        Dependency[] RoleBinding { get; }
        Collaboration Type { get; }
        }
    }