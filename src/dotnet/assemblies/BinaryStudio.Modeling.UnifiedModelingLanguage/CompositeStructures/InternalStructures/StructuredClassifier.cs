using System.Collections.Generic;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface StructuredClassifier : Classifier
        {
        /// <summary>
        /// The Properties owned by the <see cref="StructuredClassifier"/>.
        /// {ordered, subsets <see cref="Classifier.Attribute"/>,subsets <see cref="Role"/>,subsets <see cref="Namespace.OwnedMember"/>}.
        /// </summary>
        IList<Property> OwnedAttribute { get; }
        Connector[] OwnedConnector { get; }
        Property[] Part { get; }
        ConnectableElement[] Role { get; }
        }
    }