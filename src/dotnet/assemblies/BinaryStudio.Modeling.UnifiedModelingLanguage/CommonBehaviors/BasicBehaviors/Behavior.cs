using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Behavior : Class
        {
        BehavioredClassifier Context { get; }
        Boolean IsReentrant { get; }
        Parameter[] OwnedParameter { get; }
        Constraint[] Postcondition { get; }
        Constraint[] Precondition { get; }
        Behavior[] RedefinedBehavior { get; }
        BehavioralFeature Specification { get; }
        }
    }