namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Action : NamedElement
        {
        Constraint[] LocalPostcondition { get; }
        Constraint[] LocalPrecondition { get; }
        }
    }