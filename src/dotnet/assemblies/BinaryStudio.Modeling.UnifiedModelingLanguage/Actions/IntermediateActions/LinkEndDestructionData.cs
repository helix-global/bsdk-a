using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface LinkEndDestructionData : LinkEndData
        {
        InputPin DestroyAt { get; }
        Boolean IsDestroyDuplicates { get; }
        }
    }