namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ConnectableElementTemplateParameter : TemplateParameter
        {
        ConnectableElement ParameteredElement { get; }
        }
    }