using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EStructuredClassifier : EClassifier, StructuredClassifier
        {
        public EStructuredClassifier(String identifer)
            : base(identifer)
            {
            }

        public IList<Property> OwnedAttribute  { get {
            return new ReadOnlyCollection<Property>(
                OwnedElement.
                    OfType<Property>().
                    ToArray());
            }}

        public Connector[] OwnedConnector { get; }
        public Property[] Part { get; }
        public ConnectableElement[] Role { get; }
        }
    }