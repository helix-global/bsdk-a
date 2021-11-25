using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;

namespace BinaryStudio.Security.Cryptography.Xml {

    internal class CanonicalXml {
        private readonly CanonicalXmlDocument m_c14nDoc;
        private readonly C14NAncestralNamespaceContextManager m_ancMgr;

        // private static String defaultXPathWithoutComments = "(//. | //@* | //namespace::*)[not(self::comment())]";
        // private static String defaultXPathWithoutComments = "(//. | //@* | //namespace::*)";
        // private static String defaultXPathWithComments = "(//. | //@* | //namespace::*)";
        // private static String defaultXPathWithComments = "(//. | //@* | //namespace::*)";

        internal CanonicalXml(Stream inputStream, Boolean includeComments, XmlResolver resolver, String strBaseUri) {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            m_c14nDoc = new CanonicalXmlDocument(true, includeComments) {XmlResolver = resolver};
            m_c14nDoc.Load(Utils.PreProcessStreamInput(inputStream, resolver, strBaseUri));
            m_ancMgr = new C14NAncestralNamespaceContextManager();
        }

        internal CanonicalXml(XmlDocument document, XmlResolver resolver) : this(document, resolver, false) {}
        internal CanonicalXml(XmlDocument document, XmlResolver resolver, Boolean includeComments) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            m_c14nDoc = new CanonicalXmlDocument(true, includeComments) {XmlResolver = resolver};
            m_c14nDoc.Load(new XmlNodeReader(document));
            m_ancMgr = new C14NAncestralNamespaceContextManager();
        }

        internal CanonicalXml(XmlNodeList nodeList, XmlResolver resolver, Boolean includeComments) {
            if (nodeList == null)
                throw new ArgumentNullException(nameof(nodeList));

            var doc = Utils.GetOwnerDocument(nodeList);
            if (doc == null)
                throw new ArgumentException("nodeList");

            m_c14nDoc = new CanonicalXmlDocument(false, includeComments) {XmlResolver = resolver};
            m_c14nDoc.Load(new XmlNodeReader(doc));
            m_ancMgr = new C14NAncestralNamespaceContextManager();

            MarkInclusionStateForNodes(nodeList, doc, m_c14nDoc);
        }

        static void MarkNodeAsIncluded(XmlNode node) {
            if (node is ICanonicalizableNode r)
               r.IsInNodeSet = true;
        }

        private static void MarkInclusionStateForNodes(XmlNodeList nodeList, XmlDocument inputRoot, XmlDocument root) {
            var elementList = new CanonicalXmlNodeList();
            var elementListCanonical = new CanonicalXmlNodeList();
            elementList.Add(inputRoot);
            elementListCanonical.Add(root);
            var index = 0;

            do {
                var currentNode = elementList[index];
                var currentNodeCanonical = elementListCanonical[index];
                var childNodes = currentNode.ChildNodes;
                var childNodesCanonical = currentNodeCanonical.ChildNodes;
                for (var i = 0; i < childNodes.Count; i++) {
                    elementList.Add(childNodes[i]);
                    elementListCanonical.Add(childNodesCanonical[i]);

                    if (Utils.NodeInList(childNodes[i], nodeList)) {
                        MarkNodeAsIncluded(childNodesCanonical[i]);
                    }

                    var attribNodes = childNodes[i].Attributes;
                    if (attribNodes != null) {
                        for (var j = 0; j < attribNodes.Count; j++) {
                            if (Utils.NodeInList(attribNodes[j], nodeList)) {
                                MarkNodeAsIncluded(childNodesCanonical[i].Attributes.Item(j));
                            }
                        }
                    }
                }
                index++;
            } while (index < elementList.Count);
        }

        internal Byte[] GetBytes() {
            var sb = new StringBuilder();
            m_c14nDoc.Write(sb, DocPosition.BeforeRootElement, m_ancMgr);
            var utf8 = new UTF8Encoding(false);
            return utf8.GetBytes(sb.ToString());
        }

        internal Byte[] GetDigestedBytes(IHashAlgorithm hash) {
            m_c14nDoc.WriteHash(hash, DocPosition.BeforeRootElement, m_ancMgr);
            hash.TransformFinalBlock(new Byte[0], 0, 0);
            var res = (Byte[]) hash.Hash.Clone();
            // reinitialize the hash so it is still usable after the call
            hash.Initialize();
            return res;
        }
    }

    // the stack of currently active NamespaceFrame contexts. this
    // object also maintains the inclusive prefix list in a tokenized form.
    internal class C14NAncestralNamespaceContextManager : AncestralNamespaceContextManager {
        internal C14NAncestralNamespaceContextManager () {}

        private void GetNamespaceToRender(String nsPrefix, SortedList attrListToRender, SortedList nsListToRender, Hashtable nsLocallyDeclared) {
            foreach (var a in nsListToRender.GetKeyList()) {
                if (Utils.HasNamespacePrefix((XmlAttribute) a, nsPrefix))
                    return;
            }
            foreach (var a in attrListToRender.GetKeyList()) {
                if (((XmlAttribute) a).LocalName.Equals(nsPrefix))
                    return;
            }

            var local = (XmlAttribute) nsLocallyDeclared[nsPrefix];
            var rAncestral = GetNearestRenderedNamespaceWithMatchingPrefix(nsPrefix, out var rDepth);
            if(local != null) {
                if(Utils.IsNonRedundantNamespaceDecl(local, rAncestral)) {
                    nsLocallyDeclared.Remove(nsPrefix);
                    if (Utils.IsXmlNamespaceNode(local))
                        attrListToRender.Add(local, null);
                    else
                        nsListToRender.Add(local, null);
                }
            } else {
                var uAncestral = GetNearestUnrenderedNamespaceWithMatchingPrefix(nsPrefix, out var uDepth);
                if (uAncestral != null && uDepth > rDepth && Utils.IsNonRedundantNamespaceDecl(uAncestral, rAncestral)) {
                    if(Utils.IsXmlNamespaceNode(uAncestral))
                        attrListToRender.Add(uAncestral, null);
                    else
                        nsListToRender.Add(uAncestral,null);
                }
            }
        }

        internal override void GetNamespacesToRender (XmlElement element, SortedList attrListToRender, SortedList nsListToRender, Hashtable nsLocallyDeclared) {
            XmlAttribute attrib;
            var attrs = new Object[nsLocallyDeclared.Count];
            nsLocallyDeclared.Values.CopyTo(attrs,0);
            foreach(var a in attrs) {
                attrib = (XmlAttribute) a;
                var rAncestral = GetNearestRenderedNamespaceWithMatchingPrefix(Utils.GetNamespacePrefix(attrib), out var rDepth);
                if(Utils.IsNonRedundantNamespaceDecl(attrib, rAncestral)) {
                    nsLocallyDeclared.Remove(Utils.GetNamespacePrefix(attrib));
                    if (Utils.IsXmlNamespaceNode(attrib))
                        attrListToRender.Add(attrib, null);
                    else
                        nsListToRender.Add(attrib, null);
                }
            }

            for (var i = m_ancestorStack.Count - 1; i >= 0; i--) {
                foreach (var a in GetScopeAt(i).GetUnrendered().Values) {
                    attrib = (XmlAttribute) a;
                    if (attrib != null)
                        GetNamespaceToRender(Utils.GetNamespacePrefix(attrib), attrListToRender, nsListToRender, nsLocallyDeclared);
                }
            }
        }

        internal override void TrackNamespaceNode(XmlAttribute attr, SortedList nsListToRender, Hashtable nsLocallyDeclared) {
            nsLocallyDeclared.Add(Utils.GetNamespacePrefix(attr), attr);
        }

        internal override void TrackXmlNamespaceNode(XmlAttribute attr, SortedList nsListToRender, SortedList attrListToRender, Hashtable nsLocallyDeclared) {
            nsLocallyDeclared.Add(Utils.GetNamespacePrefix(attr), attr);
        }
    }
}
