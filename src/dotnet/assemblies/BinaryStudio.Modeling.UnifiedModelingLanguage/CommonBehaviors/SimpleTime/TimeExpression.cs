namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TimeExpression : ValueSpecification
        {
        ValueSpecification Expr { get; }
        Observation[] Observation { get; }
        }
    }