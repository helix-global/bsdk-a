using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Action : ActivityNode
        {
        Boolean IsLocallyReentrant { get; }
        }
    }