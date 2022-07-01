using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface OpaqueBehavior : Behavior
        {
        String[] Body { get; }
        String[] Language { get; }
        }
    }