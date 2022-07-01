using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Parameter : TypedElement, MultiplicityElement
        {
        String Default { get; }
        ValueSpecification DefaultValue { get; }
        ParameterDirectionKind Direction { get; }
        Operation Operation { get; }
        }
    }