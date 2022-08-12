using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ObjectFlow
        {
        Boolean IsMulticast { get; }
        Boolean IsMultireceive { get; }
        Behavior Selection { get; }
        Behavior Transformation { get; }
        }
    }