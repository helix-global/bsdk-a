namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface RedefinableElement : NamedElement
        {
        Classifier[] RedefinitionContext { get; }
        }
    }