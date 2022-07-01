using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface LinkEndCreationData : LinkEndData
        {
        InputPin InsertAt { get; }
        Boolean IsReplaceAll { get; }
        }
    }