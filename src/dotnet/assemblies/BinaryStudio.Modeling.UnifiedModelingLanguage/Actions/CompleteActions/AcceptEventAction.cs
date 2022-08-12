using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface AcceptEventAction : Action
        {
        Boolean IsUnmarshall { get; }
        OutputPin[] Result { get; }
        Trigger[] Trigger { get; }
        }
    }