using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface LiteralString : LiteralSpecification
        {
        String Value { get; }
        }
    }