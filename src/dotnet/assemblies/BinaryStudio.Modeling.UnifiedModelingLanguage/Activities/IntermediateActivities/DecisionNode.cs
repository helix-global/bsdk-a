namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DecisionNode : ControlNode
        {
        Behavior DecisionInput { get; }
        ObjectFlow DecisionInputFlow { get; }
        }
    }