namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface UnmarshallAction : Action
        {
        InputPin Object { get; }
        OutputPin[] Result { get; }
        Classifier UnmarshallType { get; }
        }
    }