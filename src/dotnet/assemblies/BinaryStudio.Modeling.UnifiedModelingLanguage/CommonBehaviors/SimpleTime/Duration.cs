namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Duration : ValueSpecification
        {
        ValueSpecification Expr { get; }
        Observation[] Observation { get; }
        }
    }