namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InstanceValue : ValueSpecification
        {
        InstanceSpecification Instance { get; }
        }
    }