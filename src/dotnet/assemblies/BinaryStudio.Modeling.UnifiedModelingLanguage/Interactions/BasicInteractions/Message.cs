namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Message : NamedElement
        {
        ValueSpecification[] Argument { get; }
        Connector Connector { get; }
        Interaction Interaction { get; }
        MessageKind MessageKind { get; }
        MessageSort MessageSort { get; }
        MessageEnd ReceiveEvent { get; }
        MessageEnd SendEvent { get; }
        NamedElement Signature { get; }
        }
    }