namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface WriteVariableAction : VariableAction
        {
        InputPin Value { get; }
        }
    }