namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StringExpression : TemplateableElement, Expression
        {
        StringExpression OwningExpression { get; }
        StringExpression[] SubExpression { get; }
        }
    }