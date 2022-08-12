namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface GeneralOrdering : NamedElement
        {
        OccurrenceSpecification After { get; }
        OccurrenceSpecification Before { get; }
        }
    }