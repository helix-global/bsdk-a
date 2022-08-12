namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface LoopNode : StructuredActivityNode
        {
        OutputPin[] BodyOutput { get; }
        OutputPin[] LoopVariable { get; }
        InputPin[] LoopVariableInput { get; }
        OutputPin[] Result { get; }
        }
    }