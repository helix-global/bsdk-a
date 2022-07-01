namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface LinkEndData : Element
        {
        Property End { get; }
        InputPin Value { get; }
        }
    }