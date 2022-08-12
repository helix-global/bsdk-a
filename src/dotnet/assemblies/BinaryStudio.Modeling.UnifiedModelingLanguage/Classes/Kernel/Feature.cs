using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Feature : RedefinableElement
        {
        Classifier[] FeaturingClassifier { get; }
        Boolean IsStatic { get; }
        }
    }