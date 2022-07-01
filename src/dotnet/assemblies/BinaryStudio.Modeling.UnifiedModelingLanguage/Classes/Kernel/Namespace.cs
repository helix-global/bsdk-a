namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Namespace : NamedElement
        {
        ElementImport[] ElementImport { get; }
        PackageableElement[] ImportedMember { get; }
        NamedElement[] Member { get; }
        NamedElement[] OwnedMember { get; }
        Constraint[] OwnedRule { get; }
        PackageImport[] PackageImport { get; }
        }
    }