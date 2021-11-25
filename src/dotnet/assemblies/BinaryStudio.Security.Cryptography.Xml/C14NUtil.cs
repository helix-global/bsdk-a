using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Linq;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml {

    // the current rendering position in document
    internal enum DocPosition {
        BeforeRootElement,
        InRootElement,
        AfterRootElement
    }

    // the interface to be implemented by all subclasses of XmlNode
    // that have to provide node subsetting and canonicalization features.
    internal interface ICanonicalizableNode {
        Boolean IsInNodeSet {
            get;
            set;
        }

        void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc);
        void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc);
    }

    // the central dispatcher for canonicalization writes. not all node classes
    // implement ICanonicalizableNode; so a manual dispatch is sometimes necessary.
    internal class CanonicalizationDispatcher {
        private CanonicalizationDispatcher() {}

        public static void Write(XmlNode node, StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (node is ICanonicalizableNode r) {
                r.Write(strBuilder, docPos, anc);
            } else {
                WriteGenericNode(node, strBuilder, docPos, anc);
            }
        }

        public static void WriteGenericNode(XmlNode node, StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var childNodes = node.ChildNodes;
            foreach (XmlNode childNode in childNodes) {
                Write(childNode, strBuilder, docPos, anc);
            }
        }

        public static void WriteHash(XmlNode node, IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (node is ICanonicalizableNode r) {
                r.WriteHash(hash, docPos, anc);
            } else {
                WriteHashGenericNode(node, hash, docPos, anc);
            }
        }

        public static void WriteHashGenericNode(XmlNode node, IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var childNodes = node.ChildNodes;
            foreach (XmlNode childNode in childNodes) {
                WriteHash(childNode, hash, docPos, anc);
            }
        }
    }

    // all input types eventually lead to the creation of an XmlDocument document
    // of this type. it maintains the node subset state and performs output rendering during canonicalization
    internal class CanonicalXmlDocument : XmlDocument, ICanonicalizableNode {
        readonly Boolean m_defaultNodeSetInclusionState;
        readonly Boolean m_includeComments;

        public CanonicalXmlDocument(Boolean defaultNodeSetInclusionState, Boolean includeComments) : base() {
            PreserveWhitespace = true;
            m_includeComments = includeComments;
            IsInNodeSet = m_defaultNodeSetInclusionState = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            docPos = DocPosition.BeforeRootElement;
            foreach (XmlNode childNode in ChildNodes) {
                if (childNode.NodeType == XmlNodeType.Element) {
                    CanonicalizationDispatcher.Write(childNode, strBuilder, DocPosition.InRootElement, anc);
                    docPos = DocPosition.AfterRootElement;
                } else {
                    CanonicalizationDispatcher.Write(childNode, strBuilder, docPos, anc);
                }
            }
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            docPos = DocPosition.BeforeRootElement;
            foreach (XmlNode childNode in ChildNodes) {
                if (childNode.NodeType == XmlNodeType.Element) {
                    CanonicalizationDispatcher.WriteHash(childNode, hash, DocPosition.InRootElement, anc);
                    docPos = DocPosition.AfterRootElement;
                } else {
                    CanonicalizationDispatcher.WriteHash(childNode, hash, docPos, anc);
                }
            }
        }

        public override XmlElement CreateElement(String prefix, String localName, String namespaceURI) {
            return new CanonicalXmlElement(prefix, localName, namespaceURI, this, m_defaultNodeSetInclusionState);
        }

        public override XmlAttribute CreateAttribute(String prefix, String localName, String namespaceURI) {
            return new CanonicalXmlAttribute(prefix, localName, namespaceURI, this, m_defaultNodeSetInclusionState);
        }

        protected override XmlAttribute CreateDefaultAttribute(String prefix, String localName, String namespaceURI) {
            return new CanonicalXmlAttribute(prefix, localName, namespaceURI, this, m_defaultNodeSetInclusionState);
        }

        public override XmlText CreateTextNode(String text) {
            return new CanonicalXmlText(text, this, m_defaultNodeSetInclusionState);
        }

        public override XmlWhitespace CreateWhitespace(String prefix) {
            return new CanonicalXmlWhitespace(prefix, this, m_defaultNodeSetInclusionState);
        }

        public override XmlSignificantWhitespace CreateSignificantWhitespace(String text) {
            return new CanonicalXmlSignificantWhitespace(text, this, m_defaultNodeSetInclusionState);
        }

        public override XmlProcessingInstruction CreateProcessingInstruction(String target, String data) {
            return new CanonicalXmlProcessingInstruction(target, data, this, m_defaultNodeSetInclusionState);
        }

        public override XmlComment CreateComment(String data) {
            return new CanonicalXmlComment(data, this, m_defaultNodeSetInclusionState, m_includeComments);
        }

        public override XmlEntityReference CreateEntityReference(String name) {
            return new CanonicalXmlEntityReference(name, this, m_defaultNodeSetInclusionState);
        }

        public override XmlCDataSection CreateCDataSection(String data) {
            return new CanonicalXmlCDataSection(data, this, m_defaultNodeSetInclusionState);
        }
    }

    // the class that provides node subset state and canonicalization function to XmlElement
    internal class CanonicalXmlElement : XmlElement, ICanonicalizableNode {
        public CanonicalXmlElement(String prefix, String localName, String namespaceURI, XmlDocument doc, Boolean defaultNodeSetInclusionState)
            : base(prefix, localName, namespaceURI, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            var nsLocallyDeclared = new Hashtable();
            var nsListToRender = new SortedList(new NamespaceSortOrder());
            var attrListToRender = new SortedList(new AttributeSortOrder());

            var attrList = Attributes;
            if (attrList != null) {
                foreach (XmlAttribute attr in attrList) {
                    if (((CanonicalXmlAttribute) attr).IsInNodeSet || Utils.IsNamespaceNode(attr) || Utils.IsXmlNamespaceNode(attr)) {
                        if (Utils.IsNamespaceNode(attr)) {
                            anc.TrackNamespaceNode(attr, nsListToRender, nsLocallyDeclared);
                        }
                        else if (Utils.IsXmlNamespaceNode(attr)) {
                            anc.TrackXmlNamespaceNode(attr, nsListToRender, attrListToRender, nsLocallyDeclared);
                        } 
                        else if (IsInNodeSet) {
                            attrListToRender.Add(attr, null);
                        }
                    }
                }
            }

            if (!Utils.IsCommittedNamespace(this, Prefix, NamespaceURI)) {
                var name = ((Prefix.Length > 0) ? "xmlns" + ":" + Prefix : "xmlns");
                var nsattrib = OwnerDocument.CreateAttribute(name);
                nsattrib.Value = NamespaceURI;
                anc.TrackNamespaceNode(nsattrib, nsListToRender, nsLocallyDeclared);
            }

            if (IsInNodeSet) {
                anc.GetNamespacesToRender(this, attrListToRender, nsListToRender, nsLocallyDeclared);

                strBuilder.Append("<" + Name);
                foreach (var attr in nsListToRender.GetKeyList()) {
                    (attr as CanonicalXmlAttribute).Write(strBuilder, docPos, anc);
                }
                foreach (var attr in attrListToRender.GetKeyList()) {
                    (attr as CanonicalXmlAttribute).Write(strBuilder, docPos, anc);
                }
                strBuilder.Append(">");
            }

            anc.EnterElementContext();
            anc.LoadUnrenderedNamespaces(nsLocallyDeclared);
            anc.LoadRenderedNamespaces(nsListToRender);

            var childNodes = ChildNodes;
            foreach (XmlNode childNode in childNodes) {
                CanonicalizationDispatcher.Write(childNode, strBuilder, docPos, anc);
            }

            anc.ExitElementContext();

            if (IsInNodeSet) {
                strBuilder.Append("</" + Name + ">");
            }
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            var nsLocallyDeclared = new Hashtable();
            var nsListToRender = new SortedList(new NamespaceSortOrder());
            var attrListToRender = new SortedList(new AttributeSortOrder());
            var utf8 = new UTF8Encoding(false);
            Byte[] rgbData;

            var attrList = Attributes;
            if (attrList != null) {
                foreach (XmlAttribute attr in attrList) {
                    if (((CanonicalXmlAttribute) attr).IsInNodeSet || Utils.IsNamespaceNode(attr) || Utils.IsXmlNamespaceNode(attr)) {
                        if (Utils.IsNamespaceNode(attr)) {
                            anc.TrackNamespaceNode(attr, nsListToRender, nsLocallyDeclared);
                        }
                        else if (Utils.IsXmlNamespaceNode(attr)) {
                            anc.TrackXmlNamespaceNode(attr, nsListToRender, attrListToRender, nsLocallyDeclared);
                        }
                        else if(IsInNodeSet) {
                            attrListToRender.Add(attr, null);
                        }
                    }
                }
            }

            if (!Utils.IsCommittedNamespace(this, Prefix, NamespaceURI)) {
                var name = ((Prefix.Length > 0) ? "xmlns" + ":" + Prefix : "xmlns");
                var nsattrib = OwnerDocument.CreateAttribute(name);
                nsattrib.Value = NamespaceURI;
                anc.TrackNamespaceNode(nsattrib, nsListToRender, nsLocallyDeclared);
            }

            if (IsInNodeSet) {
                anc.GetNamespacesToRender(this, attrListToRender, nsListToRender, nsLocallyDeclared);
                rgbData = utf8.GetBytes("<" + Name);
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
                foreach (var attr in nsListToRender.GetKeyList().OfType<CanonicalXmlAttribute>()) {
                    attr.WriteHash(hash, docPos, anc);
                }
                foreach (var attr in attrListToRender.GetKeyList().OfType<CanonicalXmlAttribute>()) {
                    attr.WriteHash(hash, docPos, anc);
                }
                rgbData = utf8.GetBytes(">");
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }

            anc.EnterElementContext();
            anc.LoadUnrenderedNamespaces(nsLocallyDeclared);
            anc.LoadRenderedNamespaces(nsListToRender);

            var childNodes = ChildNodes;
            foreach (XmlNode childNode in childNodes) {
                CanonicalizationDispatcher.WriteHash(childNode, hash, docPos, anc);
            }

            anc.ExitElementContext();

            if (IsInNodeSet) {
                rgbData = utf8.GetBytes("</" + Name + ">");
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }

    // the class that provides node subset state and canonicalization function to XmlAttribute
    internal class CanonicalXmlAttribute : XmlAttribute, ICanonicalizableNode {
        public CanonicalXmlAttribute(String prefix, String localName, String namespaceURI, XmlDocument doc, Boolean defaultNodeSetInclusionState)
            : base(prefix, localName, namespaceURI, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            strBuilder.Append(" " + Name + "=\"");
            strBuilder.Append(Utils.EscapeAttributeValue(Value));
            strBuilder.Append("\"");
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            var utf8 = new UTF8Encoding(false);
            var rgbData = utf8.GetBytes(" " + Name + "=\"");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes(Utils.EscapeAttributeValue(Value));
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes("\"");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
        }
    }

    // the class that provides node subset state and canonicalization function to XmlText
    internal class CanonicalXmlText : XmlText, ICanonicalizableNode {
        public CanonicalXmlText(String strData, XmlDocument doc, Boolean defaultNodeSetInclusionState)
            : base(strData, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet)
                strBuilder.Append(Utils.EscapeTextData(Value));
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet) {
                var utf8 = new UTF8Encoding(false);
                var rgbData = utf8.GetBytes(Utils.EscapeTextData(Value));
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }

    // the class that provides node subset state and canonicalization function to XmlWhitespace
    internal class CanonicalXmlWhitespace : XmlWhitespace, ICanonicalizableNode {
        public CanonicalXmlWhitespace(String strData, XmlDocument doc, Boolean defaultNodeSetInclusionState)
            : base(strData, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet && docPos == DocPosition.InRootElement)
                strBuilder.Append(Utils.EscapeWhitespaceData(Value));
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet && docPos == DocPosition.InRootElement) {
                var utf8 = new UTF8Encoding(false);
                var rgbData = utf8.GetBytes(Utils.EscapeWhitespaceData(Value));
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }

    // the class that provides node subset state and canonicalization function to XmlSignificantWhitespace
    internal class CanonicalXmlSignificantWhitespace : XmlSignificantWhitespace, ICanonicalizableNode {
        public CanonicalXmlSignificantWhitespace(String strData, XmlDocument doc, Boolean defaultNodeSetInclusionState)
            : base(strData, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet && docPos == DocPosition.InRootElement)
                strBuilder.Append(Utils.EscapeWhitespaceData(Value));
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet && docPos == DocPosition.InRootElement) {
                var utf8 = new UTF8Encoding(false);
                var rgbData = utf8.GetBytes(Utils.EscapeWhitespaceData(Value));
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }

    // the class that provides node subset state and canonicalization function to XmlComment
    internal class CanonicalXmlComment : XmlComment, ICanonicalizableNode {
        public CanonicalXmlComment(String comment, XmlDocument doc, Boolean defaultNodeSetInclusionState, Boolean includeComments)
            : base(comment, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
            IncludeComments = includeComments;
        }

        public Boolean IsInNodeSet { get; set; }

        public Boolean IncludeComments { get; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (!IsInNodeSet || !IncludeComments)
                return;

            if (docPos == DocPosition.AfterRootElement)
                strBuilder.Append((Char) 10);
            strBuilder.Append("<!--");
            strBuilder.Append(Value);
            strBuilder.Append("-->");
            if (docPos == DocPosition.BeforeRootElement)
                strBuilder.Append((Char) 10);
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (!IsInNodeSet || !IncludeComments)
                return;

            var utf8 = new UTF8Encoding(false);
            var rgbData = utf8.GetBytes("(char) 10");
            if (docPos == DocPosition.AfterRootElement)
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes("<!--");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes(Value);
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes("-->");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            if (docPos == DocPosition.BeforeRootElement) {
                rgbData = utf8.GetBytes("(char) 10");
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }



    // the class that provides node subset state and canonicalization function to XmlProcessingInstruction
    internal class CanonicalXmlProcessingInstruction : XmlProcessingInstruction, ICanonicalizableNode {
        public CanonicalXmlProcessingInstruction(String target, String data, XmlDocument doc, Boolean defaultNodeSetInclusionState)
            : base(target, data, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (!IsInNodeSet)
                return;

            if (docPos == DocPosition.AfterRootElement)
                strBuilder.Append((Char) 10);
            strBuilder.Append("<?");
            strBuilder.Append(Name);
            if ((Value != null) && (Value.Length > 0))
                strBuilder.Append(" " + Value);
            strBuilder.Append("?>");
            if (docPos == DocPosition.BeforeRootElement)
                strBuilder.Append((Char) 10);
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (!IsInNodeSet)
                return;

            var utf8 = new UTF8Encoding(false);
            Byte[] rgbData;
            if (docPos == DocPosition.AfterRootElement) {
                rgbData = utf8.GetBytes("(char) 10");
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
            rgbData = utf8.GetBytes("<?");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes((Name));
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            if ((Value != null) && (Value.Length > 0)) {
                rgbData = utf8.GetBytes(" " + Value);
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
            rgbData = utf8.GetBytes("?>");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            if (docPos == DocPosition.BeforeRootElement) {
                rgbData = utf8.GetBytes("(char) 10");
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }



    // the class that provides node subset state and canonicalization function to XmlEntityReference
    internal class CanonicalXmlEntityReference : XmlEntityReference, ICanonicalizableNode {
        public CanonicalXmlEntityReference(String name, XmlDocument doc, Boolean defaultNodeSetInclusionState)
            : base(name, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet)
                CanonicalizationDispatcher.WriteGenericNode(this, strBuilder, docPos, anc);
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet)
                CanonicalizationDispatcher.WriteHashGenericNode(this, hash, docPos, anc);
        }
    }

    // the class that provides node subset state and canonicalization function to XmlCDataSection
    internal class CanonicalXmlCDataSection: XmlCDataSection, ICanonicalizableNode {
        public CanonicalXmlCDataSection(String data, XmlDocument doc, Boolean defaultNodeSetInclusionState) : base(data, doc) {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public Boolean IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet)
                strBuilder.Append(Utils.EscapeCData(Data));
        }

        public void WriteHash(IHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc) {
            if (IsInNodeSet) {
                var utf8 = new UTF8Encoding(false);
                var rgbData = utf8.GetBytes(Utils.EscapeCData(Data));
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }

    internal class CanonicalXmlNodeList : XmlNodeList, IList {
        private readonly ArrayList m_nodeArray;

        internal CanonicalXmlNodeList() {
            m_nodeArray = new ArrayList();
        }

        public override XmlNode Item(Int32 index) {
            return (XmlNode) m_nodeArray[index];
        }

        public override IEnumerator GetEnumerator() {
            return m_nodeArray.GetEnumerator();
        }

        public override Int32 Count {
            get { return m_nodeArray.Count; }
        }

        // IList methods
        public Int32 Add(Object value) {
            if (!(value is XmlNode))
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(value));
            return m_nodeArray.Add(value);
        }

        public void Clear() {
            m_nodeArray.Clear();
        }

        public Boolean Contains(Object value) {
            return m_nodeArray.Contains(value);
        }

        public Int32 IndexOf(Object value) {
            return m_nodeArray.IndexOf(value);
        }

        public void Insert(Int32 index, Object value) {
            if (!(value is XmlNode)) 
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(value));
            m_nodeArray.Insert(index,value);
        }

        public void Remove(Object value) {
            m_nodeArray.Remove(value);
        }

        public void RemoveAt(Int32 index) {
            m_nodeArray.RemoveAt(index);
        }

        public Boolean IsFixedSize {
            get { return m_nodeArray.IsFixedSize; }
        }

        public Boolean IsReadOnly {
            get { return m_nodeArray.IsReadOnly; }
        }

        Object IList.this[Int32 index] {
            get { return m_nodeArray[index]; }
            set { 
                if (!(value is XmlNode)) 
                    throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(value));
                m_nodeArray[index] = value;
            }
        }

        public void CopyTo(Array array, Int32 index) {
            m_nodeArray.CopyTo(array, index);
        }

        public Object SyncRoot {
            get { return m_nodeArray.SyncRoot; }
        }

        public Boolean IsSynchronized {
            get { return m_nodeArray.IsSynchronized; }
        }
    }

    // This class does lexicographic sorting by NamespaceURI first and then by LocalName.
    internal class AttributeSortOrder : IComparer {
        internal AttributeSortOrder() {}

        public Int32 Compare(Object a, Object b) {
            var nodeA = a as XmlNode;
            var nodeB = b as XmlNode;
            if ((a == null) || (b == null))
                throw new ArgumentException();
            var namespaceCompare = String.CompareOrdinal(nodeA.NamespaceURI, nodeB.NamespaceURI);
            if (namespaceCompare != 0) return namespaceCompare;
            return String.CompareOrdinal(nodeA.LocalName, nodeB.LocalName);
        }
    }

    internal class NamespaceSortOrder : IComparer {
        internal NamespaceSortOrder() {}

        public Int32 Compare(Object a, Object b) {
            var nodeA = a as XmlNode;
            var nodeB = b as XmlNode;
            if ((a == null) || (b == null))
                throw new ArgumentException();
            var nodeAdefault = Utils.IsDefaultNamespaceNode(nodeA);
            var nodeBdefault = Utils.IsDefaultNamespaceNode(nodeB);
            if (nodeAdefault && nodeBdefault) return 0;
            if (nodeAdefault) return -1;
            if (nodeBdefault) return 1;
            return String.CompareOrdinal(nodeA.LocalName, nodeB.LocalName);
        }
    }

    // the namespaces context corresponding to one XmlElement. the rendered list contains the namespace nodes that are actually
    // rendered to the canonicalized output. the unrendered list contains the namespace nodes that are in the node set and have
    // the XmlElement as the owner, but are not rendered.
    internal class NamespaceFrame {
        private Hashtable m_rendered = new Hashtable();
        private Hashtable m_unrendered = new Hashtable();

        internal NamespaceFrame() {}

        internal void AddRendered(XmlAttribute attr) {
            m_rendered.Add(Utils.GetNamespacePrefix(attr), attr);
        }

        internal XmlAttribute GetRendered(String nsPrefix) {
            return (XmlAttribute) m_rendered[nsPrefix];
        }

        internal void AddUnrendered(XmlAttribute attr) {
            m_unrendered.Add(Utils.GetNamespacePrefix(attr), attr);
        }

        internal XmlAttribute GetUnrendered(String nsPrefix) {
            return (XmlAttribute) m_unrendered[nsPrefix];
        }

        internal Hashtable GetUnrendered() {
            return m_unrendered;
        }
    }

    internal abstract class AncestralNamespaceContextManager {
        internal ArrayList m_ancestorStack = new ArrayList();

        internal NamespaceFrame GetScopeAt(Int32 i) {
            return (NamespaceFrame) m_ancestorStack[i];
        }

        internal NamespaceFrame GetCurrentScope() {
            return GetScopeAt(m_ancestorStack.Count - 1);
        }

        protected XmlAttribute GetNearestRenderedNamespaceWithMatchingPrefix(String nsPrefix, out Int32 depth) {
            depth = -1;
            for (var i = m_ancestorStack.Count - 1; i >= 0; i--) {
                XmlAttribute attr;
                if ((attr = GetScopeAt(i).GetRendered(nsPrefix)) != null) {
                    depth = i;
                    return attr;
                    }
                }
            return null;
            }

        protected XmlAttribute GetNearestUnrenderedNamespaceWithMatchingPrefix(String nsPrefix, out Int32 depth) {
            depth = -1;
            for (var i = m_ancestorStack.Count - 1; i >= 0; i--) {
                XmlAttribute attr;
                if ((attr = GetScopeAt(i).GetUnrendered(nsPrefix)) != null) {
                    depth = i;
                    return attr;
                    }
                }
            return null;
            }

        internal void EnterElementContext() {
            m_ancestorStack.Add(new NamespaceFrame());
        }

        internal void ExitElementContext() {
            m_ancestorStack.RemoveAt(m_ancestorStack.Count - 1);
        }

        internal abstract void TrackNamespaceNode(XmlAttribute attr, SortedList nsListToRender, Hashtable nsLocallyDeclared);
        internal abstract void TrackXmlNamespaceNode(XmlAttribute attr, SortedList nsListToRender, SortedList attrListToRender, Hashtable nsLocallyDeclared);
        internal abstract void GetNamespacesToRender (XmlElement element, SortedList attrListToRender, SortedList nsListToRender, Hashtable nsLocallyDeclared);

        internal void LoadUnrenderedNamespaces(Hashtable nsLocallyDeclared) {
            var attrs = new Object[nsLocallyDeclared.Count];
            nsLocallyDeclared.Values.CopyTo(attrs, 0);
            foreach (var attr in attrs) {
                AddUnrendered((XmlAttribute) attr);
            }
        }

        internal void LoadRenderedNamespaces(SortedList nsRenderedList) {
            foreach (var attr in nsRenderedList.GetKeyList()) {
                AddRendered((XmlAttribute) attr);
            }
        }

        internal void AddRendered(XmlAttribute attr) {
            GetCurrentScope().AddRendered(attr);
        }

        internal void AddUnrendered(XmlAttribute attr) {
            GetCurrentScope().AddUnrendered(attr);
        }
    }
}
