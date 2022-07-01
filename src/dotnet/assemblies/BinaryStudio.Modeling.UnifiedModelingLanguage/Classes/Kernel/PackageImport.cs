namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface PackageImport : DirectedRelationship
        {
        Package ImportedPackage { get; }
        Namespace ImportingNamespace { get; }
        VisibilityKind Visibility { get; }
        }
    }