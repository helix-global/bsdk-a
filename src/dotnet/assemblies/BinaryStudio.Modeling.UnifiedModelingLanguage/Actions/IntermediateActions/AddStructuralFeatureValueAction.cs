using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface AddStructuralFeatureValueAction : WriteStructuralFeatureAction
        {
        InputPin InsertAt { get; }
        Boolean IsReplaceAll { get; }
        }
    }