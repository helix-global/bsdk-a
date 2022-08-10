using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DurationConstraint : IntervalConstraint
        {
        Boolean FirstEvent { get; }
        new DurationInterval Specification { get; }
        }
    }