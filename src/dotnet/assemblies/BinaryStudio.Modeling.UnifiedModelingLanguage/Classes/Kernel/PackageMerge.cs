namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface PackageMerge : DirectedRelationship
        {
        Package MergedPackage { get; }
        Package ReceivingPackage { get; }
        }
    }