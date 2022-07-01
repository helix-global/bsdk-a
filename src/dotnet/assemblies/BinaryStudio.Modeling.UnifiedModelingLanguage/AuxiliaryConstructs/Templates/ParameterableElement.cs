namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ParameterableElement : Element
        {
        TemplateParameter OwningTemplateParameter { get; }
        TemplateParameter TemplateParameter { get; }
        }
    }