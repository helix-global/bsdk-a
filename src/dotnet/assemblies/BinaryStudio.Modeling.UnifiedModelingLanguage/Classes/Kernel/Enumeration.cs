namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Enumeration : DataType
        {
        EnumerationLiteral[] OwnedLiteral { get; }
        }
    }