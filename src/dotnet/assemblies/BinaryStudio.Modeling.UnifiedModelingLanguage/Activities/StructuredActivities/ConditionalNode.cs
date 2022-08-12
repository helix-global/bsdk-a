using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ConditionalNode : StructuredActivityNode
        {
        Clause[] Clause { get; }
        Boolean IsAssured { get; }
        Boolean IsDeterminate { get; }
        }
    }