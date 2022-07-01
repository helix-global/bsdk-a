using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Class : BehavioredClassifier
        {
        Boolean IsActive { get; }
        Reception[] OwnedReception { get; }
        }
    }