namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface QualifierValue : Element
        {
        Property Qualifier { get; }
        InputPin Value { get; }
        }
    }