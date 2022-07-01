namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface InteractionFragment : NamedElement
        {
        Lifeline[] Covered { get; }
        Interaction EnclosingInteraction { get; }
        GeneralOrdering[] GeneralOrdering { get; }
        }
    }