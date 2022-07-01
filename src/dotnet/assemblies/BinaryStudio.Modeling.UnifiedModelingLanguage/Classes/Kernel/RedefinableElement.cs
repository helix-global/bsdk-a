using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface RedefinableElement : NamedElement
        {
        Boolean IsLeaf { get; }
        RedefinableElement[] RedefinedElement { get; }
        //Classifier[] RedefinitionContext { get; }
        }
    }