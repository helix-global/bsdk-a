namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ProtocolStateMachine : StateMachine
        {
        ProtocolConformance[] Conformance { get; }
        }
    }