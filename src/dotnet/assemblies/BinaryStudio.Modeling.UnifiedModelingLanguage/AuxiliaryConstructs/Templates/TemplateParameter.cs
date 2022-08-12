namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TemplateParameter : Element
        {
        ParameterableElement Default { get; }
        ParameterableElement OwnedDefault { get; }
        ParameterableElement OwnedParameteredElement { get; }
        ParameterableElement ParameteredElement { get; }
        TemplateSignature Signature { get; }
        }
    }