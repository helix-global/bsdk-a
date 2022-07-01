namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ConditionalNode : StructuredActivityNode
        {
        OutputPin[] Result { get; }
        }
    }