using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface BehavioralFeature
        {
        Boolean IsAbstract { get; }
        Behavior[] Method { get; }
        }
    }