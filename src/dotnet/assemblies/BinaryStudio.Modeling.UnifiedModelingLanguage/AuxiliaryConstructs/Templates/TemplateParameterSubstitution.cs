namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TemplateParameterSubstitution : Element
        {
        ParameterableElement Actual { get; }
        TemplateParameter Formal { get; }
        ParameterableElement OwnedActual { get; }
        TemplateBinding TemplateBinding { get; }
        }
    }