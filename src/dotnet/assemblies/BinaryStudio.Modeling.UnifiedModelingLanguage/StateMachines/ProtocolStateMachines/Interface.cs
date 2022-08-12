namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Interface : Classifier
        {
        ProtocolStateMachine Protocol { get; }
        }
    }