using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DurationConstraint : IntervalConstraint
        {
        Boolean FirstEvent { get; }
        DurationInterval Specification { get; }
        }
    }