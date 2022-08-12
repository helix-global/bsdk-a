namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Classifier : Namespace, ParameterableElement, TemplateableElement
        {
        new RedefinableTemplateSignature OwnedTemplateSignature { get; }
        new ClassifierTemplateParameter TemplateParameter { get; }
        }
    }