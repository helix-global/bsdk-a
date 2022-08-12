namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReadLinkObjectEndQualifierAction : Action
        {
        InputPin Object { get; }
        Property Qualifier { get; }
        OutputPin Result { get; }
        }
    }