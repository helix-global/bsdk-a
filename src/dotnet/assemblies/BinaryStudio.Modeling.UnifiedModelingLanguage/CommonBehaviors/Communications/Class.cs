using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Class : BehavioredClassifier
        {
        /// <summary>
        /// Determines whether an object specified by this Class is active or not. If true, then the owning Class is referred
        /// to as an active Class. If false, then such a Class is referred to as a passive Class.
        /// </summary>
        Boolean IsActive { get;set; }
        Reception[] OwnedReception { get; }
        }
    }