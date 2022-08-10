namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ConnectableElement : ParameterableElement
        {
        new ConnectableElementTemplateParameter TemplateParameter { get; }
        }
    }