namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface RedefinableTemplateSignature : RedefinableElement, TemplateSignature
        {
        Classifier Classifier { get; }
        RedefinableTemplateSignature[] ExtendedSignature { get; }
        TemplateParameter[] InheritedParameter { get; }
        }
    }