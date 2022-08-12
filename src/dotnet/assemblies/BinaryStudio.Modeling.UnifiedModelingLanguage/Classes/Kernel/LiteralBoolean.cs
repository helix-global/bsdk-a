using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface LiteralBoolean : LiteralSpecification
        {
        Boolean Value { get; }
        }
    }