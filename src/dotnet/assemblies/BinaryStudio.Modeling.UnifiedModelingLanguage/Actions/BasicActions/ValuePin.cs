namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ValuePin : InputPin
        {
        ValueSpecification Value { get; }
        }
    }