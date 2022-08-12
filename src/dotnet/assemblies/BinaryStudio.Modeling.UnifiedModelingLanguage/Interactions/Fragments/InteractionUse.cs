namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InteractionUse : InteractionFragment
        {
        Gate[] ActualGate { get; }
        ValueSpecification[] Argument { get; }
        Interaction RefersTo { get; }
        ValueSpecification ReturnValue { get; }
        Property ReturnValueRecipient { get; }
        }
    }