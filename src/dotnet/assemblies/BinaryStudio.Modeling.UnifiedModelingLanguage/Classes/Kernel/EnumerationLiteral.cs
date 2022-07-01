namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface EnumerationLiteral : InstanceSpecification
        {
        Enumeration Classifier { get; }
        Enumeration Enumeration { get; }
        }
    }