using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TimeObservation : Observation
        {
        NamedElement Event { get; }
        Boolean FirstEvent { get; }
        }
    }