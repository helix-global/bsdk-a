using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReclassifyObjectAction : Action
        {
        Boolean IsReplaceAll { get; }
        Classifier[] NewClassifier { get; }
        InputPin Object { get; }
        Classifier[] OldClassifier { get; }
        }
    }