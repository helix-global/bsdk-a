namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ParameterSet : NamedElement
        {
        Constraint[] Condition { get; }
        Parameter[] Parameter { get; }
        }
    }