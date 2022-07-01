namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TimeInterval : Interval
        {
        TimeExpression Max { get; }
        TimeExpression Min { get; }
        }
    }