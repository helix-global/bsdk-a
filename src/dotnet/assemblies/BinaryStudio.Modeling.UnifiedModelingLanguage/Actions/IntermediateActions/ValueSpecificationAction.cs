namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ValueSpecificationAction : Action
        {
        OutputPin Result { get; }
        ValueSpecification Value { get; }
        }
    }