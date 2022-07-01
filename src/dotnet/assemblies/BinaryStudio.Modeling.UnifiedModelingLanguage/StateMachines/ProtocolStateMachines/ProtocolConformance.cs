namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ProtocolConformance : DirectedRelationship
        {
        ProtocolStateMachine GeneralMachine { get; }
        ProtocolStateMachine SpecificMachine { get; }
        }
    }