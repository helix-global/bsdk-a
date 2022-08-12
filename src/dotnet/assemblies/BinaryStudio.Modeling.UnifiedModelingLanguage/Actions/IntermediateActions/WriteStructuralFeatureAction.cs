namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface WriteStructuralFeatureAction : StructuralFeatureAction
        {
        OutputPin Result { get; }
        InputPin Value { get; }
        }
    }