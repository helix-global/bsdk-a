namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface TemplateBinding : DirectedRelationship
        {
        TemplateableElement BoundElement { get; }
        TemplateParameterSubstitution[] ParameterSubstitution { get; }
        TemplateSignature Signature { get; }
        }
    }