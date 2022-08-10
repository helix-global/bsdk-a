namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface EnumerationLiteral : InstanceSpecification
        {
        new Enumeration Classifier { get; }
        Enumeration Enumeration { get; }
        }
    }