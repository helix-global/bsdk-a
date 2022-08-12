using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface RemoveStructuralFeatureValueAction : WriteStructuralFeatureAction
        {
        Boolean IsRemoveDuplicates { get; }
        InputPin RemoveAt { get; }
        }
    }