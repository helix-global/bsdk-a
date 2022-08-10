namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface OccurrenceSpecification : InteractionFragment
        {
        new Lifeline Covered { get; }
        GeneralOrdering[] ToAfter { get; }
        GeneralOrdering[] ToBefore { get; }
        }
    }