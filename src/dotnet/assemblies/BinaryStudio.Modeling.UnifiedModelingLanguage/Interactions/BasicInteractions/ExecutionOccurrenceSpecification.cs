namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ExecutionOccurrenceSpecification : OccurrenceSpecification
        {
        ExecutionSpecification Execution { get; }
        }
    }