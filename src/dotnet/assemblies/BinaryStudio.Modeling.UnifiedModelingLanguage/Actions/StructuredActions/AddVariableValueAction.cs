using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface AddVariableValueAction : WriteVariableAction
        {
        InputPin InsertAt { get; }
        Boolean IsReplaceAll { get; }
        }
    }