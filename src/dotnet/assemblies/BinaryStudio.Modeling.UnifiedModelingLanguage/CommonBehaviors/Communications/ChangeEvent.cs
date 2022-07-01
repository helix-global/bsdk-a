namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ChangeEvent : Event
        {
        ValueSpecification ChangeExpression { get; }
        }
    }