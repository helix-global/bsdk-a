namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Classifier : Namespace, ParameterableElement, TemplateableElement
        {
        RedefinableTemplateSignature OwnedTemplateSignature { get; }
        ClassifierTemplateParameter TemplateParameter { get; }
        }
    }