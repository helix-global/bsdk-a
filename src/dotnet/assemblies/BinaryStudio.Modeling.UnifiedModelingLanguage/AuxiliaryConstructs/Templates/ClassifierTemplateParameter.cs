using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ClassifierTemplateParameter : TemplateParameter
        {
        Boolean AllowSubstitutable { get; }
        Classifier[] ConstrainingClassifier { get; }
        Classifier ParameteredElement { get; }
        }
    }