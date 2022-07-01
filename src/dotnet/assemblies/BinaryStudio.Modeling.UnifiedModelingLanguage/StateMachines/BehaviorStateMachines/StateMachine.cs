namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StateMachine : Behavior
        {
        Pseudostate[] ConnectionPoint { get; }
        StateMachine[] ExtendedStateMachine { get; }
        Region[] Region { get; }
        State[] SubmachineState { get; }
        }
    }