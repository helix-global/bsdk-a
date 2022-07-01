namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ProtocolTransition : Transition
        {
        Constraint PostCondition { get; }
        Constraint PreCondition { get; }
        Operation[] Referred { get; }
        }
    }