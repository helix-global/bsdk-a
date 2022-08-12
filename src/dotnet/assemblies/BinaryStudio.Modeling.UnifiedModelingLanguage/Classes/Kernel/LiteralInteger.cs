namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    using Integer = System.Int64;
    public interface LiteralInteger : LiteralSpecification
        {
        Integer Value { get; }
        }
    }