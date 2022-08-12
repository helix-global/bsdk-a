using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface JoinNode : ControlNode
        {
        Boolean IsCombineDuplicate { get; }
        ValueSpecification JoinSpec { get; }
        }
    }