using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EDataType : EClassifier, DataType
        {
        public EDataType(String identifer)
            : base(identifer)
            {
            }

        public Property[] OwnedAttribute { get; }
        public Operation[] OwnedOperation { get; }
        }
    }