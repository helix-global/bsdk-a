namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface InvocationAction : Action
        {
        InputPin[] Argument { get; }
        }
    }