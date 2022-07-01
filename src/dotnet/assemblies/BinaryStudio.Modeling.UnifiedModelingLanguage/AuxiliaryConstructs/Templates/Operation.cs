namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Operation : TemplateableElement, ParameterableElement
        {
        OperationTemplateParameter TemplateParameter { get; }
        }
    }