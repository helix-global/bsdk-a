namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Transition : Namespace, RedefinableElement
        {
        Region Container { get; }
        Behavior Effect { get; }
        Constraint Guard { get; }
        TransitionKind Kind { get; }
        Transition RedefinedTransition { get; }
        new Classifier RedefinitionContext { get; }
        Vertex Source { get; }
        Vertex Target { get; }
        Trigger[] Trigger { get; }
        }
    }