using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReadIsClassifiedObjectAction : Action
        {
        Classifier Classifier { get; }
        Boolean IsDirect { get; }
        InputPin Object { get; }
        OutputPin Result { get; }
        }
    }