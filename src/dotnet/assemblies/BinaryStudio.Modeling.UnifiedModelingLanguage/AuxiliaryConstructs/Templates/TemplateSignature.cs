namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TemplateSignature : Element
        {
        TemplateParameter[] OwnedParameter { get; }
        TemplateParameter[] Parameter { get; }
        TemplateableElement Template { get; }
        }
    }