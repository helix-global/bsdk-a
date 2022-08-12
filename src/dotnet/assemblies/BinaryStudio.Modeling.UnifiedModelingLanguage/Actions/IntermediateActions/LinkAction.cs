namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface LinkAction : Action
        {
        LinkEndData[] EndData { get; }
        InputPin[] InputValue { get; }
        }
    }