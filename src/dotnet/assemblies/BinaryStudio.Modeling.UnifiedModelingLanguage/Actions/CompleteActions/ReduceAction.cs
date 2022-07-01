using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ReduceAction : Action
        {
        InputPin Collection { get; }
        Boolean IsOrdered { get; }
        Behavior Reducer { get; }
        OutputPin Result { get; }
        }
    }