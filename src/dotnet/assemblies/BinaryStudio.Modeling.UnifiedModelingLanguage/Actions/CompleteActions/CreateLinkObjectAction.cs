namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CreateLinkObjectAction : CreateLinkAction
        {
        OutputPin Result { get; }
        }
    }