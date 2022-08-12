namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TemplateableElement : Element
        {
        TemplateSignature OwnedTemplateSignature { get; }
        TemplateBinding[] TemplateBinding { get; }
        }
    }