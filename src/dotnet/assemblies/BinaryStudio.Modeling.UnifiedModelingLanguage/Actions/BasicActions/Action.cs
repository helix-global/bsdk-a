namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Action : NamedElement
        {
        Classifier Context { get; }
        InputPin[] Input { get; }
        OutputPin[] Output { get; }
        }
    }