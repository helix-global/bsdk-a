namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TimeInterval : Interval
        {
        new TimeExpression Max { get; }
        new TimeExpression Min { get; }
        }
    }