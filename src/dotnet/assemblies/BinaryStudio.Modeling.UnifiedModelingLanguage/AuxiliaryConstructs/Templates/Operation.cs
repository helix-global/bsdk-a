namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Operation : TemplateableElement, ParameterableElement
        {
        new OperationTemplateParameter TemplateParameter { get; }
        }
    }