namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReadLinkObjectEndAction : Action
        {
        Property End { get; }
        InputPin Object { get; }
        OutputPin Result { get; }
        }
    }