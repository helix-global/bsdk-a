using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Classifier : Type, RedefinableElement, Namespace
        {
        Property[] Attribute { get; }
        Feature[] Feature { get; }
        Classifier[] General { get; }
        Generalization[] Generalization { get; }
        NamedElement[] InheritedMember { get; }
        Boolean IsAbstract { get; }
        Boolean IsFinalSpecialization { get; }
        Classifier[] RedefinedClassifier { get; }
        }
    }