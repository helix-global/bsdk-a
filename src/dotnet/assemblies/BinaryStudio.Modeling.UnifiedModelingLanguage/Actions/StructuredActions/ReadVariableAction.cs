namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReadVariableAction : VariableAction
        {
        OutputPin Result { get; }
        }
    }