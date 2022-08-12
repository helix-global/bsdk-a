namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ConnectionPointReference : Vertex
        {
        Pseudostate[] Entry { get; }
        Pseudostate[] Exit { get; }
        State State { get; }
        }
    }