namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface NamedElement : Element
        {
        StringExpression NameExpression { get; }
        }
    }