namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface BehaviorExecutionSpecification : ExecutionSpecification
        {
        Behavior Behavior { get; }
        }
    }