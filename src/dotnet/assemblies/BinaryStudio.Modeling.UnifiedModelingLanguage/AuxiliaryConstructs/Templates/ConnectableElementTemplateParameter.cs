namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ConnectableElementTemplateParameter : TemplateParameter
        {
        new ConnectableElement ParameteredElement { get; }
        }
    }