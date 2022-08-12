namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface InteractionConstraint : Constraint
        {
        ValueSpecification Maxint { get; }
        ValueSpecification Minint { get; }
        }
    }