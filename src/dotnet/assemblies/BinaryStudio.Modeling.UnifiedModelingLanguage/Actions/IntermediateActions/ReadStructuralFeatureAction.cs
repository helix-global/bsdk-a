namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReadStructuralFeatureAction : StructuralFeatureAction
        {
        OutputPin Result { get; }
        }
    }