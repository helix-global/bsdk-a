namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface OccurrenceSpecification : InteractionFragment
        {
        Lifeline Covered { get; }
        GeneralOrdering[] ToAfter { get; }
        GeneralOrdering[] ToBefore { get; }
        }
    }