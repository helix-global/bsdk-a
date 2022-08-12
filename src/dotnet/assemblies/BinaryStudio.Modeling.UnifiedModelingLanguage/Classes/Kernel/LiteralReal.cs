namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    using Real = System.Double;
    public interface LiteralReal : LiteralSpecification
        {
        Real Value { get; }
        }
    }