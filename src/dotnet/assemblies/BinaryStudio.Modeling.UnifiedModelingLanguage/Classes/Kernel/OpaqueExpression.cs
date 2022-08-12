using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface OpaqueExpression : ValueSpecification
        {
        String[] Body { get; }
        String[] Language { get; }
        }
    }