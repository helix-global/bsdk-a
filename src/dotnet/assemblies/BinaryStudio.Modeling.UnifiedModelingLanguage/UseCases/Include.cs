namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Include : NamedElement, DirectedRelationship
        {
        UseCase Addition { get; }
        UseCase IncludingCase { get; }
        }
    }