namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TestIdentityAction : Action
        {
        InputPin First { get; }
        OutputPin Result { get; }
        InputPin Second { get; }
        }
    }