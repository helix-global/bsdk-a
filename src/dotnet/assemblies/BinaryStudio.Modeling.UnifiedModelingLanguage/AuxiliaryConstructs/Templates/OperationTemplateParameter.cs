namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface OperationTemplateParameter : TemplateParameter
        {
        new Operation ParameteredElement { get; }
        }
    }