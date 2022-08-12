namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StateInvariant : InteractionFragment
        {
        new Lifeline Covered { get; }
        Constraint Invariant { get; }
        }
    }