using System;
using System.Collections.Generic;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Class : Classifier
        {
        /// <summary>
        /// If true, the <see cref="Class"/> does not provide a complete declaration and cannot be instantiated. An abstract Class is
        /// typically used as a target of Associations or Generalizations.
        /// </summary>
        new Boolean IsAbstract { get;set; }
        Classifier[] NestedClassifier { get; }
        /// <summary>
        /// The attributes (i.e., the Properties) owned by the Class.
        /// {ordered, subsets <see cref="Classifier.Attribute"/>, subsets <see cref="Namespace.OwnedMember"/>,
        /// redefines <see cref="StructuredClassifier.OwnedAttribute"/>} (opposite <see cref="Property.Class"/>).
        /// </summary>
        new IList<Property> OwnedAttribute { get; }
        Operation[] OwnedOperation { get; }
        Class[] SuperClass { get; }
        }
    }