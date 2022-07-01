using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Class : Classifier
        {
        Boolean IsAbstract { get; }
        Classifier[] NestedClassifier { get; }
        Property[] OwnedAttribute { get; }
        Operation[] OwnedOperation { get; }
        Class[] SuperClass { get; }
        }
    }