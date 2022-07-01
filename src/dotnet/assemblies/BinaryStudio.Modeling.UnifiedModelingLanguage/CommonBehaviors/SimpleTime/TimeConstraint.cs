using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TimeConstraint : IntervalConstraint
        {
        Boolean FirstEvent { get; }
        TimeInterval Specification { get; }
        }
    }