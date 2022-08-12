namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Vertex : NamedElement
        {
        Region Container { get; }
        Transition[] Incoming { get; }
        Transition[] Outgoing { get; }
        }
    }