namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface IntervalConstraint : Constraint
        {
        Interval Specification { get; }
        }
    }