namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InteractionOperand : Namespace, InteractionFragment
        {
        InteractionFragment[] Fragment { get; }
        InteractionConstraint Guard { get; }
        }
    }