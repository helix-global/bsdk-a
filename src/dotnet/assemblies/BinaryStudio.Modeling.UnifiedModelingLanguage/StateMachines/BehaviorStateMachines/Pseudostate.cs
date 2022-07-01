namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Pseudostate : Vertex
        {
        PseudostateKind Kind { get; }
        State State { get; }
        StateMachine StateMachine { get; }
        }
    }