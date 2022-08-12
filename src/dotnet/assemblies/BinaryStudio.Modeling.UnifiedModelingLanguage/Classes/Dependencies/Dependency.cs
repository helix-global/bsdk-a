namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Dependency : DirectedRelationship, PackageableElement
        {
        NamedElement[] Client { get; }
        NamedElement[] Supplier { get; }
        }
    }