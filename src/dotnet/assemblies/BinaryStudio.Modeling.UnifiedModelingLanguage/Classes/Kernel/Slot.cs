namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Slot : Element
        {
        StructuralFeature DefiningFeature { get; }
        InstanceSpecification OwningInstance { get; }
        ValueSpecification[] Value { get; }
        }
    }