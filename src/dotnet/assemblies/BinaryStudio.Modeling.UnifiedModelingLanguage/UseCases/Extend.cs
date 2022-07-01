namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Extend : DirectedRelationship, NamedElement
        {
        Constraint Condition { get; }
        UseCase ExtendedCase { get; }
        UseCase Extension { get; }
        ExtensionPoint[] ExtensionLocation { get; }
        }
    }