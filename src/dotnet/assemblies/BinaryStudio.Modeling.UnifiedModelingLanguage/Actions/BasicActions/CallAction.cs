using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CallAction : InvocationAction
        {
        Boolean IsSynchronous { get; }
        OutputPin[] Result { get; }
        }
    }