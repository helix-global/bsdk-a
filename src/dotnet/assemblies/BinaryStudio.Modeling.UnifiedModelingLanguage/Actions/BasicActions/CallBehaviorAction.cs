namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CallBehaviorAction : CallAction
        {
        Behavior Behavior { get; }
        }
    }