namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ClearAssociationAction : Action
        {
        Association Association { get; }
        InputPin Object { get; }
        }
    }