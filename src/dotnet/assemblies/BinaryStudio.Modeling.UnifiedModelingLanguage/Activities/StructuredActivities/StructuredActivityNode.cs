namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface StructuredActivityNode : Namespace, ExecutableNode, ActivityGroup
        {
        new Activity Activity { get; }
        ActivityNode[] Node { get; }
        Variable[] Variable { get; }
        }
    }