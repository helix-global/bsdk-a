namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DurationInterval : Interval
        {
        new Duration Max { get; }
        new Duration Min { get; }
        }
    }