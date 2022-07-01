namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StateInvariant : InteractionFragment
        {
        Lifeline Covered { get; }
        Constraint Invariant { get; }
        }
    }