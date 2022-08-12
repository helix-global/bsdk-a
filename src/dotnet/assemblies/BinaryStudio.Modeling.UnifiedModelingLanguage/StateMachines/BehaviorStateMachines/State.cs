using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface State : RedefinableElement, Namespace, Vertex
        {
        ConnectionPointReference[] Connection { get; }
        Pseudostate[] ConnectionPoint { get; }
        Trigger[] DeferrableTrigger { get; }
        Behavior DoActivity { get; }
        Behavior Entry { get; }
        Behavior Exit { get; }
        Boolean IsComposite { get; }
        Boolean IsOrthogonal { get; }
        Boolean IsSimple { get; }
        Boolean IsSubmachineState { get; }
        State RedefinedState { get; }
        new Classifier RedefinitionContext { get; }
        Region[] Region { get; }
        Constraint StateInvariant { get; }
        StateMachine Submachine { get; }
        }
    }