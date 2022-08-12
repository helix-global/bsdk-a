using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EElement : Element
        {
        public String Identifier { get; }
        public IList<Comment> OwnedComment { get {
            return new ReadOnlyCollection<Comment>(
                OwnedElement.
                    OfType<Comment>().
                    ToArray());
            }}

        public IList<Element> OwnedElement { get; }

        public Element Owner { get; }

        public EElement(String identifer)
            {
            Identifier = Identifier;
            OwnedElement = new List<Element>();
            }
        }
    }