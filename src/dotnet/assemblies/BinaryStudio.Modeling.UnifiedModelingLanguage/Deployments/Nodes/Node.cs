namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Node : Class, DeploymentTarget
        {
        Node[] NestedNode { get; }
        }
    }