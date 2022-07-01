namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Region : RedefinableElement, Namespace
        {
        Region ExtendedRegion { get; }
        Classifier RedefinitionContext { get; }
        State State { get; }
        StateMachine StateMachine { get; }
        Vertex[] Subvertex { get; }
        Transition[] Transition { get; }
        }
    }