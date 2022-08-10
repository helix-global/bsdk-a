using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EClass : EStructuredClassifier, Class
        {
        public EClass(String identifer)
            : base(identifer)
            {
            }

        public Port[] OwnedPort { get; }
        public Behavior ClassifierBehavior { get; }
        public Behavior[] OwnedBehavior { get; }
        public InterfaceRealization[] InterfaceRealization { get; }
        public Boolean IsActive { get;set; }
        public Reception[] OwnedReception { get; }
        public Classifier[] NestedClassifier { get; }
        public Operation[] OwnedOperation { get; }
        public Class[] SuperClass { get; }
        }
    }