using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface ObjectNode : TypedElement
        {
        State[] InState { get; }
        Boolean IsControlType { get; }
        ObjectNodeOrderingKind Ordering { get; }
        Behavior Selection { get; }
        ValueSpecification UpperBound { get; }
        }
    }