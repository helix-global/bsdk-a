using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DestroyObjectAction : Action
        {
        Boolean IsDestroyLinks { get; }
        Boolean IsDestroyOwnedObjects { get; }
        InputPin Target { get; }
        }
    }