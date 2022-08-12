namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Interaction : Behavior
        {
        Gate[] FormalGate { get; }
        }
    }