namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StartObjectBehaviorAction : CallAction
        {
        InputPin Object { get; }
        }
    }