using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Activity : Behavior
        {
        ActivityEdge[] Edge { get; }
        Boolean IsReadOnly { get; }
        }
    }