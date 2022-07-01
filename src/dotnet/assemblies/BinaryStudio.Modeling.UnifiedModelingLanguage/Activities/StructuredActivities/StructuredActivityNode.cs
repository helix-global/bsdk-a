namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface StructuredActivityNode : Namespace, ExecutableNode, ActivityGroup
        {
        Activity Activity { get; }
        ActivityNode[] Node { get; }
        Variable[] Variable { get; }
        }
    }