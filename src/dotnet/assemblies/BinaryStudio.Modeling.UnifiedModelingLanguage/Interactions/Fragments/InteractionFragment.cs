namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface InteractionFragment : NamedElement
        {
        InteractionOperand EnclosingOperand { get; }
        }
    }