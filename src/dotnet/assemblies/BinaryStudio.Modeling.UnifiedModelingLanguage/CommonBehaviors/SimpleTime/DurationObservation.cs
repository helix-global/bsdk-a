using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DurationObservation : Observation
        {
        NamedElement Event { get; }
        Boolean FirstEvent { get; }
        }
    }