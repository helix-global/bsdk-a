namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Interval : ValueSpecification
        {
        ValueSpecification Max { get; }
        ValueSpecification Min { get; }
        }
    }