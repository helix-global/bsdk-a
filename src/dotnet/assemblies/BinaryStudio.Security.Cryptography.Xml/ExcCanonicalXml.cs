using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;

namespace BinaryStudio.Security.Cryptography.Xml
    {
    internal class ExcCanonicalXml {
        private CanonicalXmlDocument m_c14nDoc;
        private ExcAncestralNamespaceContextManager m_ancMgr;

        internal ExcCanonicalXml(Stream inputStream, Boolean includeComments, String inclusiveNamespacesPrefixList, XmlResolver resolver, String strBaseUri) {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            m_c14nDoc = new CanonicalXmlDocument(true, includeComments);
            m_c14nDoc.XmlResolver = resolver;
            m_c14nDoc.Load(Utils.PreProcessStreamInput(inputStream, resolver, strBaseUri));
            m_ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
        }

        internal ExcCanonicalXml(XmlDocument document, Boolean includeComments, String inclusiveNamespacesPrefixList, XmlResolver resolver) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            m_c14nDoc = new CanonicalXmlDocument(true, includeComments);
            m_c14nDoc.XmlResolver = resolver;
            m_c14nDoc.Load(new XmlNodeReader(document));
            m_ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
        }

        internal ExcCanonicalXml(XmlNodeList nodeList, Boolean includeComments, String inclusiveNamespacesPrefixList, XmlResolver resolver) {
            if (nodeList == null)
                throw new ArgumentNullException(nameof(nodeList));

            var doc = Utils.GetOwnerDocument(nodeList);
            if (doc == null)
                throw new ArgumentException("nodeList");

            m_c14nDoc = new CanonicalXmlDocument(false, includeComments);
            m_c14nDoc.XmlResolver = resolver;
            m_c14nDoc.Load(new XmlNodeReader(doc));
            m_ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);

            MarkInclusionStateForNodes(nodeList, doc, m_c14nDoc);
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

        private static void MarkInclusionStateForNodes(XmlNodeList nodeList, XmlDocument inputRoot, XmlDocument root) {
            var elementList = new CanonicalXmlNodeList();
            var elementListCanonical = new CanonicalXmlNodeList();
            elementList.Add(inputRoot);
            elementListCanonical.Add(root);
            var index = 0;

            do {
                var currentNode = (XmlNode) elementList[index];
                var currentNodeCanonical = (XmlNode) elementListCanonical[index];
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

        static void MarkNodeAsIncluded(XmlNode node) {
            if (node is ICanonicalizableNode)
               ((ICanonicalizableNode) node).IsInNodeSet = true;
        }
    }

    // the stack of currently active NamespaceFrame contexts. this
    // object also maintains the inclusive prefix list in a tokenized form.
    internal class ExcAncestralNamespaceContextManager : AncestralNamespaceContextManager {
        private Hashtable m_inclusivePrefixSet;

        internal ExcAncestralNamespaceContextManager(String inclusiveNamespacesPrefixList) {
            m_inclusivePrefixSet = Utils.TokenizePrefixListString(inclusiveNamespacesPrefixList);
        }

        private Boolean HasNonRedundantInclusivePrefix(XmlAttribute attr) {
            Int32 tmp;
            var nsPrefix = Utils.GetNamespacePrefix(attr);
            return m_inclusivePrefixSet.ContainsKey(nsPrefix) &&
                Utils.IsNonRedundantNamespaceDecl(attr, GetNearestRenderedNamespaceWithMatchingPrefix(nsPrefix, out tmp));
        }

        private void GatherNamespaceToRender(String nsPrefix, SortedList nsListToRender, Hashtable nsLocallyDeclared) {
            foreach (var a in nsListToRender.GetKeyList()) {
                if (Utils.HasNamespacePrefix((XmlAttribute) a, nsPrefix))
                    return;
            }

            Int32 rDepth;
            var local = (XmlAttribute) nsLocallyDeclared[nsPrefix];
            var rAncestral = GetNearestRenderedNamespaceWithMatchingPrefix(nsPrefix, out rDepth);

            if (local != null) {
                if (Utils.IsNonRedundantNamespaceDecl(local, rAncestral)) {
                    nsLocallyDeclared.Remove(nsPrefix);
                    nsListToRender.Add(local, null);
                }
            } else {
                Int32 uDepth;
                var uAncestral = GetNearestUnrenderedNamespaceWithMatchingPrefix(nsPrefix, out uDepth);
                if (uAncestral != null && uDepth > rDepth && Utils.IsNonRedundantNamespaceDecl(uAncestral, rAncestral)) {
                    nsListToRender.Add(uAncestral, null);
                }
            }
        }

        internal override void GetNamespacesToRender (XmlElement element, SortedList attrListToRender, SortedList nsListToRender, Hashtable nsLocallyDeclared) {                
            GatherNamespaceToRender(element.Prefix, nsListToRender, nsLocallyDeclared);
            foreach (var attr in attrListToRender.GetKeyList()) {
                var prefix = ((XmlAttribute) attr).Prefix;
                if (prefix.Length > 0)
                    GatherNamespaceToRender(prefix, nsListToRender, nsLocallyDeclared);
            }
        }
    
        internal override void TrackNamespaceNode(XmlAttribute attr, SortedList nsListToRender, Hashtable nsLocallyDeclared) {
            if(!Utils.IsXmlPrefixDefinitionNode(attr)) {
                if(HasNonRedundantInclusivePrefix(attr))
                    nsListToRender.Add(attr, null);
                else
                    nsLocallyDeclared.Add(Utils.GetNamespacePrefix(attr), attr);
            }
        }

        internal override void TrackXmlNamespaceNode(XmlAttribute attr, SortedList nsListToRender, SortedList attrListToRender, Hashtable nsLocallyDeclared) {
            // exclusive canonicalization treats Xml namespaces as simple attributes. They are not propagated.
            attrListToRender.Add(attr, null);
        }
    }
}
