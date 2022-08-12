namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StructuralFeatureAction : Action
        {
        InputPin Object { get; }
        StructuralFeature StructuralFeature { get; }
        }
    }