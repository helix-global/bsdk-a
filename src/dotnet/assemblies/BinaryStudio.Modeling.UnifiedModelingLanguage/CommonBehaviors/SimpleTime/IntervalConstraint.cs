namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface IntervalConstraint : Constraint
        {
        new Interval Specification { get; }
        }
    }