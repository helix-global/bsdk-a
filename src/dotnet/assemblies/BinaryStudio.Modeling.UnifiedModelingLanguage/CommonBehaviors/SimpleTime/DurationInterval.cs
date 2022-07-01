namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DurationInterval : Interval
        {
        Duration Max { get; }
        Duration Min { get; }
        }
    }