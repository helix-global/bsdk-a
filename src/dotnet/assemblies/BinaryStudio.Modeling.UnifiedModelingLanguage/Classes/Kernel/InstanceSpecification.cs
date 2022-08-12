namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface InstanceSpecification : PackageableElement
        {
        Classifier[] Classifier { get; }
        Slot[] Slot { get; }
        ValueSpecification Specification { get; }
        }
    }