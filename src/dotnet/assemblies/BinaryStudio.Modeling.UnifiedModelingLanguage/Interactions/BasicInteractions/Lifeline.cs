namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Lifeline : NamedElement
        {
        InteractionFragment[] CoveredBy { get; }
        Interaction Interaction { get; }
        ConnectableElement Represents { get; }
        ValueSpecification Selector { get; }
        }
    }