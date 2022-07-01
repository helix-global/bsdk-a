using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Parameter
        {
        ParameterEffectKind Effect { get; }
        Boolean IsException { get; }
        Boolean IsStream { get; }
        ParameterSet[] ParameterSet { get; }
        }
    }