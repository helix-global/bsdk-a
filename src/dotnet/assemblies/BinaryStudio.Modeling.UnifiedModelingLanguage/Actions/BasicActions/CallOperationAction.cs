namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CallOperationAction : CallAction
        {
        Operation Operation { get; }
        InputPin Target { get; }
        }
    }