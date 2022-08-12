namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ExecutionSpecification : InteractionFragment
        {
        OccurrenceSpecification Finish { get; }
        OccurrenceSpecification Start { get; }
        }
    }