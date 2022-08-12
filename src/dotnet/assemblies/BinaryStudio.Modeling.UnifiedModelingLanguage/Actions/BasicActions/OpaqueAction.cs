using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface OpaqueAction : Action
        {
        String[] Body { get; }
        InputPin[] InputValue { get; }
        String[] Language { get; }
        OutputPin[] OutputValue { get; }
        }
    }