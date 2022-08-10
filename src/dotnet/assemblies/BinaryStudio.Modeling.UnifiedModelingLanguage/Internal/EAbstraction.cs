using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EAbstraction : EDependency, Abstraction
        {
        public EAbstraction(String identifer)
            : base(identifer)
            {
            }

        public OpaqueExpression Mapping { get; }
        }
    }