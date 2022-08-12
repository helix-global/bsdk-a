using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface RemoveVariableValueAction : WriteVariableAction
        {
        Boolean IsRemoveDuplicates { get; }
        InputPin RemoveAt { get; }
        }
    }