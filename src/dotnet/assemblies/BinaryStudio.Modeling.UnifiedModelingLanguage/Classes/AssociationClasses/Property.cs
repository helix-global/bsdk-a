namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Property : StructuralFeature
        {
        Property AssociationEnd { get; }
        Property[] Qualifier { get; }
        }
    }