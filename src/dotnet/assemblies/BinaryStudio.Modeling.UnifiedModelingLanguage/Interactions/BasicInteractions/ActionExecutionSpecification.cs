namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ActionExecutionSpecification : ExecutionSpecification
        {
        Action Action { get; }
        }
    }