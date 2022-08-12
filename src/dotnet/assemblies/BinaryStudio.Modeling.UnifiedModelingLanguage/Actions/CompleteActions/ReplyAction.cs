namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReplyAction : Action
        {
        Trigger ReplyToCall { get; }
        InputPin[] ReplyValue { get; }
        InputPin ReturnInformation { get; }
        }
    }