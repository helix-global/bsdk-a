using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    using Integer = System.Int64;
    public partial interface MultiplicityElement : Element
        {
        Boolean IsOrdered { get; }
        Boolean IsUnique { get; }
        Integer Lower { get; }
        ValueSpecification LowerValue { get; }
        UnlimitedNatural Upper { get; }
        ValueSpecification UpperValue { get; }
        }
    }