namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DirectedRelationship : Relationship
        {
        Element[] Source { get; }
        Element[] Target { get; }
        }
    }