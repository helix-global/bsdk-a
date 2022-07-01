namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface SequenceNode : StructuredActivityNode
        {
        ExecutableNode[] ExecutableNode { get; }
        }
    }