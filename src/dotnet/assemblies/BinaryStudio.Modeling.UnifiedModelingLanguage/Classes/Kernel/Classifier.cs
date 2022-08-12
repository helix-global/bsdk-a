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
        /// <summary>
        /// If true, the Classifier can only be instantiated by instantiating one of its specializations. An abstract Classifier is
        /// intended to be used by other Classifiers e.g., as the target of Associations or Generalizations.
        /// </summary>
        Boolean IsAbstract { get;set; }
        Boolean IsFinalSpecialization { get; }
        Classifier[] RedefinedClassifier { get; }
        }
    }