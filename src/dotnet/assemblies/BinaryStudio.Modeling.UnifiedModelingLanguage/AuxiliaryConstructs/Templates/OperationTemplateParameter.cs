namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface OperationTemplateParameter : TemplateParameter
        {
        Operation ParameteredElement { get; }
        }
    }