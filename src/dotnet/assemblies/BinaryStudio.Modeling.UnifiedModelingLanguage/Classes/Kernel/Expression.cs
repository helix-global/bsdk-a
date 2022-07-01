using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Expression : ValueSpecification
        {
        ValueSpecification[] Operand { get; }
        String Symbol { get; }
        }
    }