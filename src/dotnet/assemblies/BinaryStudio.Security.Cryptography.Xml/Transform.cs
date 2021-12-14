using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    using CryptoStream = CryptoStream;

    // This class represents an ordered chain of transforms

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class TransformChain {
        private ArrayList m_transforms;

        public TransformChain () {
            m_transforms = new ArrayList();
        }

        public void Add (Transform transform) {
            if (transform != null) 
                m_transforms.Add(transform);
        }

        public IEnumerator GetEnumerator() {
            return m_transforms.GetEnumerator();
        }

        public Int32 Count {
            get { return m_transforms.Count; }
        }

        public Transform this[Int32 index] {
            get {
                if (index >= m_transforms.Count)
                    throw new ArgumentException( SecurityResources.GetResourceString("ArgumentOutOfRange_Index"), nameof(index));
                return (Transform) m_transforms[index];
            }
        }

        // The goal behind this method is to pump the input stream through the transforms and get back something that
        // can be hashed
        internal Stream TransformToOctetStream(Object inputObject, Type inputType, XmlResolver resolver, String baseUri) {
            var currentInput = inputObject;
            foreach (Transform transform in m_transforms) {
                if (currentInput == null || transform.AcceptsType(currentInput.GetType())) {
                    //in this case, no translation necessary, pump it through
                    transform.Resolver = resolver;
                    transform.BaseURI = baseUri;
                    transform.LoadInput(currentInput);
                    currentInput = transform.GetOutput();
                } else {
                    // We need translation 
                    // For now, we just know about Stream->{XmlNodeList,XmlDocument} and {XmlNodeList,XmlDocument}->Stream
                    if (currentInput is Stream) {
                        if (transform.AcceptsType(typeof(XmlDocument))) {
                            var currentInputStream = currentInput as Stream;
                            var doc = new XmlDocument();
                            doc.PreserveWhitespace = true;
                            var valReader = Utils.PreProcessStreamInput(currentInputStream, resolver, baseUri);
                            doc.Load(valReader);
                            transform.LoadInput(doc);
                            currentInputStream.Close();
                            currentInput = transform.GetOutput();
                            continue;
                        } else {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"));
                        }
                    } 
                    if (currentInput is XmlNodeList) {
                        if (transform.AcceptsType(typeof(Stream))) {
                            var c14n = new CanonicalXml((XmlNodeList) currentInput, resolver, false);
                            var ms = new MemoryStream(c14n.GetBytes());
                            transform.LoadInput(ms);
                            currentInput = transform.GetOutput();
                            ms.Close();
                            continue;
                        } else {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"));
                        }
                    }
                    if (currentInput is XmlDocument) {
                        if (transform.AcceptsType(typeof(Stream))) {
                            var c14n = new CanonicalXml((XmlDocument) currentInput, resolver);
                            var ms = new MemoryStream(c14n.GetBytes());
                            transform.LoadInput(ms);
                            currentInput = transform.GetOutput();
                            ms.Close();
                            continue;
                        } else {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"));
                        }
                    }
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"));
                }
            }

            // Final processing, either we already have a stream or have to canonicalize
            if (currentInput is Stream) {
                return currentInput as Stream;
            }
            if (currentInput is XmlNodeList) {
                var c14n = new CanonicalXml((XmlNodeList) currentInput, resolver, false);
                var ms = new MemoryStream(c14n.GetBytes());
                return ms;
            }
            if (currentInput is XmlDocument) {
                var c14n = new CanonicalXml((XmlDocument) currentInput, resolver);
                var ms = new MemoryStream(c14n.GetBytes());
                return ms;
            }
            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"));
        }

        internal Stream TransformToOctetStream(Stream input, XmlResolver resolver, String baseUri) {
            return TransformToOctetStream(input, typeof(Stream), resolver, baseUri);
        }

        internal Stream TransformToOctetStream(XmlDocument document, XmlResolver resolver, String baseUri) {
            return TransformToOctetStream(document, typeof(XmlDocument), resolver, baseUri);
        }

        internal XmlElement GetXml (XmlDocument document, String ns) {
            var transformsElement = document.CreateElement("Transforms", ns);
            foreach (Transform transform in m_transforms) {
                if (transform != null) {
                    // Construct the individual transform element
                    var transformElement = transform.GetXml(document);
                    if (transformElement != null)
                        transformsElement.AppendChild(transformElement);
                }
            }
            return transformsElement;
        }

        internal void LoadXml (XmlElement value) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var nsm = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

            var transformNodes = value.SelectNodes("ds:Transform", nsm);
            if (transformNodes.Count == 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidElement"), "Transforms");

            m_transforms.Clear();
            for (var i = 0; i < transformNodes.Count; ++i) {
                var transformElement = (XmlElement) transformNodes.Item(i);
                var algorithm = Utils.GetAttribute(transformElement, "Algorithm", SignedXml.XmlDsigNamespaceUrl);
                var transform = Utils.CreateFromName<Transform>(algorithm);
                if (transform == null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                // let the transform read the children of the transformElement for data
                transform.LoadInnerXml(transformElement.ChildNodes);
                m_transforms.Add(transform);
            }
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public abstract class Transform {
        internal XmlResolver m_xmlResolver;
        private Hashtable m_propagatedNamespaces;
        private XmlElement m_context;

        internal String BaseURI { get; set; }

        internal SignedXml SignedXml { get; set; }

        internal Reference Reference { get; set; }

        //
        // protected constructors
        //

        protected Transform() {}

        //
        // public properties
        //

        public String Algorithm { get; set; }

        [ComVisible(false)]
        public XmlResolver Resolver {
            // This property only has a setter. The rationale for this is that we don't have a good value
            // to return when it has not been explicitely set, as we are using XmlSecureResolver by default
            set { 
                m_xmlResolver = value; 
                ResolverSet = true;
            }

            internal get {
                return m_xmlResolver;
            }
        }

        internal Boolean ResolverSet { get; private set; }

        public abstract Type[] InputTypes {
            get;
        }

        public abstract Type[] OutputTypes {
            get;
        }

        internal Boolean AcceptsType(Type inputType) {
            if (InputTypes != null) {
                for (var i=0; i<InputTypes.Length; i++) {
                    if (inputType == InputTypes[i] || inputType.IsSubclassOf(InputTypes[i]))
                        return true;
                }
            }
            return false;
        }

        //
        // public methods
        //

        public XmlElement GetXml() {
            var document = new XmlDocument();
            document.PreserveWhitespace = true;
            return GetXml(document);
        }

        internal XmlElement GetXml (XmlDocument document) {
            return GetXml (document, "Transform");
        }

        internal XmlElement GetXml (XmlDocument document, String name) {
            var transformElement = document.CreateElement(name, SignedXml.XmlDsigNamespaceUrl);
            if (!String.IsNullOrEmpty(Algorithm))
                transformElement.SetAttribute("Algorithm", Algorithm);
            var children = GetInnerXml();
            if (children != null) {
                foreach (XmlNode node in children) {
                    transformElement.AppendChild(document.ImportNode(node, true));
                }
            }
            return transformElement;
        }

        public abstract void LoadInnerXml(XmlNodeList nodeList);

        protected abstract XmlNodeList GetInnerXml();

        public abstract void LoadInput(Object obj);

        public abstract Object GetOutput();

        public abstract Object GetOutput(Type type);

        [ComVisible(false)]
        public virtual Byte[] GetDigestedOutput(IHashAlgorithm hash) {
            return hash.Compute((Stream)GetOutput(typeof(Stream)));
        }

        [ComVisible(false)]
        public XmlElement Context {
            get {
                if (m_context != null)
                    return m_context;

                var reference = Reference;
                var signedXml = (reference == null ? SignedXml : reference.SignedXml);
                if (signedXml == null)
                    return null;

                return signedXml.m_context;
            }
            set {
                m_context = value;
            }
        }

        [ComVisible(false)]
        public Hashtable PropagatedNamespaces {
            get {
                if (m_propagatedNamespaces != null)
                    return m_propagatedNamespaces;

                var reference = Reference;
                var signedXml = (reference == null ? SignedXml : reference.SignedXml);

                // If the reference is not a Uri reference with a DataObject target, return an empty hashtable.
                if (reference != null && 
                    ((reference.ReferenceTargetType != ReferenceTargetType.UriReference) ||
                     (reference.Uri == null || reference.Uri.Length == 0 || reference.Uri[0] != '#'))) {
                    m_propagatedNamespaces = new Hashtable(0);
                    return m_propagatedNamespaces;
                }

                CanonicalXmlNodeList namespaces = null;
                if (reference != null)
                    namespaces = reference.m_namespaces;
                else if (signedXml.m_context != null)
                    namespaces = Utils.GetPropagatedAttributes(signedXml.m_context);

                // if no namespaces have been propagated, return an empty hashtable.
                if (namespaces == null) {
                    m_propagatedNamespaces = new Hashtable(0);
                    return m_propagatedNamespaces;
                }

                m_propagatedNamespaces = new Hashtable(namespaces.Count);
                foreach (XmlNode attrib in namespaces) {
                    var key = ((attrib.Prefix.Length > 0) ? attrib.Prefix + ":" + attrib.LocalName : attrib.LocalName);
                    if (!m_propagatedNamespaces.Contains(key))
                        m_propagatedNamespaces.Add(key, attrib.Value);
                }
                return m_propagatedNamespaces;
            }
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigC14NTransform : Transform {
        private CanonicalXml _cXml;
        private Boolean _includeComments;

        public XmlDsigC14NTransform() {
            Algorithm = SignedXml.XmlDsigC14NTransformUrl;
        }

        public XmlDsigC14NTransform(Boolean includeComments) {
            _includeComments = includeComments;
            Algorithm = (includeComments ? SignedXml.XmlDsigC14NWithCommentsTransformUrl : SignedXml.XmlDsigC14NTransformUrl);
        }

        public override Type[] InputTypes { get; } = { typeof(Stream), typeof(XmlDocument), typeof(XmlNodeList) };

        public override Type[] OutputTypes { get; } = { typeof(Stream) };

        public override void LoadInnerXml(XmlNodeList nodeList) {
            if (!Utils.GetAllowAdditionalSignatureNodes() && nodeList != null && nodeList.Count > 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
        }

        protected override XmlNodeList GetInnerXml() {
            return null;
        }

        public override void LoadInput(Object obj) {
            var resolver = (ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), BaseURI));
            if (obj is Stream) {
                _cXml = new CanonicalXml((Stream) obj, _includeComments, resolver, BaseURI);
                return;
            }
            if (obj is XmlDocument) {
                _cXml = new CanonicalXml((XmlDocument) obj, resolver, _includeComments);
                return;
            }
            if (obj is XmlNodeList) {
                _cXml = new CanonicalXml((XmlNodeList) obj, resolver, _includeComments);
            } 
	     else {
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(obj));            
	     }
        }

        public override Object GetOutput() {
            return new MemoryStream(_cXml.GetBytes());
        }

        public override Object GetOutput(Type type) {
            if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream))) 
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));
            return new MemoryStream(_cXml.GetBytes());
        }

        [ComVisible(false)]
        public override Byte[] GetDigestedOutput(IHashAlgorithm hash) {
            return _cXml.GetDigestedBytes(hash);
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigC14NWithCommentsTransform : XmlDsigC14NTransform {
        public XmlDsigC14NWithCommentsTransform() 
            : base(true) {
            Algorithm = SignedXml.XmlDsigC14NWithCommentsTransformUrl;
        }
    }

    // <ds:Transform Algorithm="http://www.w3.org/2001/10/xml-exc-c14n#">
    //     <ec:InclusiveNamespaces PrefixList="dsig soap #default" xmlns:ec="http://www.w3.org/2001/10/xml-exc-c14n#"/>
    // </ds:Transform>

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigExcC14NTransform : Transform {
        private Boolean _includeComments;
        private ExcCanonicalXml _excCanonicalXml;

        public XmlDsigExcC14NTransform() : this(false, null) {}

        public XmlDsigExcC14NTransform(Boolean includeComments) : this(includeComments, null) {}

        public XmlDsigExcC14NTransform(String inclusiveNamespacesPrefixList) : this(false, inclusiveNamespacesPrefixList) {}

        public XmlDsigExcC14NTransform(Boolean includeComments, String inclusiveNamespacesPrefixList) {
            _includeComments = includeComments;
            InclusiveNamespacesPrefixList = inclusiveNamespacesPrefixList;
            Algorithm = (includeComments ? SignedXml.XmlDsigExcC14NWithCommentsTransformUrl : SignedXml.XmlDsigExcC14NTransformUrl);
        }

        public String InclusiveNamespacesPrefixList { get; set; }

        public override Type[] InputTypes { get; } = { typeof(Stream), typeof(XmlDocument), typeof(XmlNodeList) };

        public override Type[] OutputTypes { get; } = { typeof(Stream) };

        public override void LoadInnerXml(XmlNodeList nodeList) {
            if (nodeList != null) {
                foreach (XmlNode n in nodeList) {
                    var e = n as XmlElement;
                    if (e != null) {
                        if (e.LocalName.Equals("InclusiveNamespaces") 
                        && e.NamespaceURI.Equals(SignedXml.XmlDsigExcC14NTransformUrl) &&
                        Utils.HasAttribute(e, "PrefixList", SignedXml.XmlDsigNamespaceUrl)) {
                            if (!Utils.VerifyAttributes(e, "PrefixList")) {
                                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                            }
                            InclusiveNamespacesPrefixList = Utils.GetAttribute(e, "PrefixList", SignedXml.XmlDsigNamespaceUrl);
                            return;
                        }
                        else if (!Utils.GetAllowAdditionalSignatureNodes()) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                        }
                    }
                }
            }
        }

        public override void LoadInput(Object obj) {
            var resolver = (ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), BaseURI));
            if (obj is Stream) {
                _excCanonicalXml = new ExcCanonicalXml((Stream) obj, _includeComments, InclusiveNamespacesPrefixList, resolver, BaseURI);
            }
            else if (obj is XmlDocument) {
                _excCanonicalXml = new ExcCanonicalXml((XmlDocument) obj, _includeComments, InclusiveNamespacesPrefixList, resolver);
            }
            else if (obj is XmlNodeList) {
                _excCanonicalXml = new ExcCanonicalXml((XmlNodeList) obj, _includeComments, InclusiveNamespacesPrefixList, resolver);
            } else
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_IncorrectObjectType"), nameof(obj));
        }

        protected override XmlNodeList GetInnerXml() {
            if (InclusiveNamespacesPrefixList == null)
                return null;
            var document = new XmlDocument();
            var element = document.CreateElement("Transform", SignedXml.XmlDsigNamespaceUrl);
            if (!String.IsNullOrEmpty(Algorithm))
                element.SetAttribute("Algorithm", Algorithm);
            var prefixListElement = document.CreateElement("InclusiveNamespaces", SignedXml.XmlDsigExcC14NTransformUrl);
            prefixListElement.SetAttribute("PrefixList", InclusiveNamespacesPrefixList);
            element.AppendChild(prefixListElement);
            return element.ChildNodes;
        }

        public override Object GetOutput() {
            return new MemoryStream(_excCanonicalXml.GetBytes());
        }

        public override Object GetOutput(Type type) {
            if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));
            return new MemoryStream(_excCanonicalXml.GetBytes());
        }

        public override Byte[] GetDigestedOutput(IHashAlgorithm hash) {
            return _excCanonicalXml.GetDigestedBytes(hash);
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigExcC14NWithCommentsTransform : XmlDsigExcC14NTransform {
        public XmlDsigExcC14NWithCommentsTransform() : base(true) {
            Algorithm = SignedXml.XmlDsigExcC14NWithCommentsTransformUrl;
        }

        public XmlDsigExcC14NWithCommentsTransform(String inclusiveNamespacesPrefixList) : base(true, inclusiveNamespacesPrefixList) {
            Algorithm = SignedXml.XmlDsigExcC14NWithCommentsTransformUrl;
        }
    }

    // A class representing conversion from Base64 using CryptoStream
    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigBase64Transform : Transform {
        private CryptoStream _cs;

        public XmlDsigBase64Transform() {
            Algorithm = SignedXml.XmlDsigBase64TransformUrl;
        }

        public override Type[] InputTypes { get; } = { typeof(Stream), typeof(XmlNodeList), typeof(XmlDocument) };

        public override Type[] OutputTypes { get; } = { typeof(Stream) };

        public override void LoadInnerXml(XmlNodeList nodeList) {
            if (!Utils.GetAllowAdditionalSignatureNodes() && nodeList != null && nodeList.Count > 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
        }

        protected override XmlNodeList GetInnerXml() {
            return null;
        }

        public override void LoadInput(Object obj) {
            if (obj is Stream) {
                LoadStreamInput((Stream) obj);
                return;
            }
            if (obj is XmlNodeList) {
                LoadXmlNodeListInput((XmlNodeList) obj);
                return;
            }
            if (obj is XmlDocument) {
                LoadXmlNodeListInput(((XmlDocument) obj).SelectNodes("//."));
                return;
            }
        }

        private void LoadStreamInput(Stream inputStream) {
            if (inputStream == null) throw new ArgumentException("obj");
            var ms = new MemoryStream();
            var buffer = new Byte[1024];
            Int32 bytesRead;
            do {
                bytesRead = inputStream.Read(buffer,0,1024);
                if (bytesRead > 0) {
                    var i = 0;
                    var j = 0;
                    while ((j < bytesRead) && (!Char.IsWhiteSpace((Char) buffer[j]))) j++;
                    i = j; j++;
                    while (j < bytesRead) {
                        if (!Char.IsWhiteSpace((Char) buffer[j])) {
                            buffer[i] = buffer[j];
                            i++;
                        }
                        j++;
                    }
                    ms.Write(buffer,0,i);
                }
            } while (bytesRead > 0);
            ms.Position = 0;
            _cs = new CryptoStream(ms, new FromBase64Transform(), CryptoStreamMode.Read);
        }

        private void LoadXmlNodeListInput(XmlNodeList nodeList) {
            var sb = new StringBuilder();
            foreach (XmlNode node in nodeList) {
                var result = node.SelectSingleNode("self::text()");
                if (result != null)
                    sb.Append(result.OuterXml);
            }
            var utf8 = new UTF8Encoding(false);
            var buffer = utf8.GetBytes(sb.ToString());
            var i = 0;
            var j = 0;
            while ((j <buffer.Length) && (!Char.IsWhiteSpace((Char) buffer[j]))) j++;
            i = j; j++;
            while (j < buffer.Length) {
                if (!Char.IsWhiteSpace((Char) buffer[j])) {
                    buffer[i] = buffer[j];
                    i++;
                }
                j++;
            }
            var ms = new MemoryStream(buffer, 0, i);
            _cs = new CryptoStream(ms, new FromBase64Transform(), CryptoStreamMode.Read);
        }

        public override Object GetOutput() {
            return _cs;
        }

        public override Object GetOutput(Type type) {
            if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));
            return _cs;
        }
    }

    // A class representing DSIG XPath Transforms

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigXPathTransform : Transform {
        private String _xpathexpr;
        private XmlDocument _document;
        private XmlNamespaceManager _nsm;

        public XmlDsigXPathTransform() {
            Algorithm = SignedXml.XmlDsigXPathTransformUrl;
        }

        public override Type[] InputTypes { get; } = { typeof(Stream), typeof(XmlNodeList), typeof(XmlDocument) };

        public override Type[] OutputTypes { get; } = { typeof(XmlNodeList) };

        public override void LoadInnerXml(XmlNodeList nodeList) {
            // XPath transform is specified by text child of first XPath child
            if (nodeList == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));

            foreach (XmlNode node in nodeList) {
                String prefix = null;
                String namespaceURI = null;
                var elem = node as XmlElement;
                if (elem != null) {
                    if (elem.LocalName == "XPath") {
                        _xpathexpr = elem.InnerXml.Trim(null);
                        var nr = new XmlNodeReader(elem);
                        var nt = nr.NameTable;
                        _nsm = new XmlNamespaceManager(nt);
                        if (!Utils.VerifyAttributes(elem, (String)null)) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                        }
                        // Look for a namespace in the attributes
                        foreach (XmlAttribute attrib in elem.Attributes) {
                            if (attrib.Prefix == "xmlns") {
                                prefix = attrib.LocalName;
                                namespaceURI = attrib.Value;
                                if (prefix == null) {
                                    prefix = elem.Prefix;
                                    namespaceURI = elem.NamespaceURI;
                                }
                                _nsm.AddNamespace(prefix, namespaceURI);
                            }
                        }
                        break;
                    }
                    else if (!Utils.GetAllowAdditionalSignatureNodes()) {
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                    }
                }
            }

            if (_xpathexpr == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
        }

        protected override XmlNodeList GetInnerXml() {
            var document = new XmlDocument();
            var element = document.CreateElement(null, "XPath", SignedXml.XmlDsigNamespaceUrl);

            if (_nsm != null) {
                // Add each of the namespaces as attributes of the element
                foreach (String prefix in _nsm) {
                    switch (prefix) {
                        // Ignore the xml namespaces
                        case "xml":
                        case "xmlns":
                            break;

                        // Other namespaces
                        default:
                            // Ignore the default namespace
                            if (prefix != null && prefix.Length > 0)
                                element.SetAttribute("xmlns:" + prefix, _nsm.LookupNamespace(prefix));
                            break;
                    }
                }
            }
            // Add the XPath as the inner xml of the element
            element.InnerXml = _xpathexpr;
            document.AppendChild(element);
            return document.ChildNodes;
        }

        public override void LoadInput(Object obj) {
            if (obj is Stream) {
                LoadStreamInput((Stream) obj);
            } else if (obj is XmlNodeList) {
                LoadXmlNodeListInput((XmlNodeList) obj);
            } else if (obj is XmlDocument) {
                LoadXmlDocumentInput((XmlDocument) obj);
            }
        }

        private void LoadStreamInput(Stream stream) {
            var resolver = (ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), BaseURI));
            var valReader = Utils.PreProcessStreamInput(stream, resolver, BaseURI);
            _document = new XmlDocument();
            _document.PreserveWhitespace = true;
            _document.Load(valReader);
        }

        private void LoadXmlNodeListInput(XmlNodeList nodeList) {
            // Use C14N to get a document
            var resolver = (ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), BaseURI));
            var c14n = new CanonicalXml((XmlNodeList) nodeList, resolver, true);
            using (var ms = new MemoryStream(c14n.GetBytes())) {
                LoadStreamInput(ms);
            }
        }

        private void LoadXmlDocumentInput(XmlDocument doc) {
            _document = doc;
        }

        public override Object GetOutput() {
            var resultNodeList = new CanonicalXmlNodeList();
            if (!String.IsNullOrEmpty(_xpathexpr)) {
                var navigator = _document.CreateNavigator();
                var it = navigator.Select("//. | //@*");

                var xpathExpr = navigator.Compile("boolean(" + _xpathexpr + ")");
                xpathExpr.SetContext(_nsm);

                while (it.MoveNext()) {
                    var node = ((IHasXmlNode) it.Current).GetNode();

                    var include = (Boolean) it.Current.Evaluate(xpathExpr);
                    if (include == true)
                        resultNodeList.Add(node);
                }

                // keep namespaces
                it = navigator.Select("//namespace::*");
                while (it.MoveNext()) {
                    var node = ((IHasXmlNode) it.Current).GetNode();
                    resultNodeList.Add(node);
                }
            }

            return resultNodeList;
        }

        public override Object GetOutput(Type type) {
            if (type != typeof(XmlNodeList) && !type.IsSubclassOf(typeof(XmlNodeList))) 
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));
            return (XmlNodeList) GetOutput();
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigXsltTransform : Transform {
        private XmlNodeList _xslNodes;
        private String _xslFragment;
        private Stream _inputStream;
        private Boolean _includeComments;

        public XmlDsigXsltTransform() {
            Algorithm = SignedXml.XmlDsigXsltTransformUrl;
        }

        public XmlDsigXsltTransform(Boolean includeComments) {
            _includeComments = includeComments;
            Algorithm = SignedXml.XmlDsigXsltTransformUrl;
        }

        public override Type[] InputTypes { get; } = { typeof(Stream), typeof(XmlDocument), typeof(XmlNodeList) };

        public override Type[] OutputTypes { get; } = { typeof(Stream) };

        public override void LoadInnerXml(XmlNodeList nodeList) {
            if (nodeList == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
            // check that the XSLT element is well formed
            XmlElement firstDataElement = null;
            var count = 0;
            foreach (XmlNode node in nodeList) {
                // ignore white spaces, but make sure only one child element is present
                if (node is XmlWhitespace) continue;
                if (node is XmlElement) {
                    if (count != 0) 
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                    firstDataElement = node as XmlElement;
                    count++;
                    continue;
                }
                // Only allow white spaces
                count++;
            }
            if (count != 1 || firstDataElement == null) 
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
            _xslNodes = nodeList;
            _xslFragment = firstDataElement.OuterXml.Trim(null);
        }

        protected override XmlNodeList GetInnerXml() {
            return _xslNodes;
        }

        public override void LoadInput(Object obj) {
            if (_inputStream != null)
                _inputStream.Close();
            _inputStream = new MemoryStream();
            if (obj is Stream) {
                _inputStream = (Stream) obj;
            }
            else if (obj is XmlNodeList) {
                var xmlDoc = new CanonicalXml((XmlNodeList) obj, null, _includeComments);
                var buffer = xmlDoc.GetBytes();
                if (buffer == null) return;
                _inputStream.Write(buffer, 0, buffer.Length);
                _inputStream.Flush();
                _inputStream.Position = 0;
            }
            else if (obj is XmlDocument) {
                var xmlDoc = new CanonicalXml((XmlDocument) obj, null, _includeComments);
                var buffer = xmlDoc.GetBytes();
                if (buffer == null) return;
                _inputStream.Write(buffer, 0, buffer.Length);
                _inputStream.Flush();
                _inputStream.Position = 0;
            }
        }

        public override Object GetOutput() {
            //  XSL transforms expose many powerful features by default:
            //  1- we need to pass a null evidence to prevent script execution.
            //  2- XPathDocument will expand entities, we don't want this, so set the resolver to null
            //  3- We don't want the document function feature of XslTransforms.

            // load the XSL Transform
            var xslt = new XslCompiledTransform();
            var settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.MaxCharactersFromEntities = Utils.GetMaxCharactersFromEntities();
            settings.MaxCharactersInDocument = Utils.GetMaxCharactersInDocument();
            using (var sr = new StringReader(_xslFragment)) {
                var readerXsl = XmlReader.Create(sr, settings, (String)null);
                xslt.Load(readerXsl, XsltSettings.Default, null);

                // Now load the input stream, XmlDocument can be used but is less efficient
                var reader = XmlReader.Create(_inputStream, settings, BaseURI);
                var inputData = new XPathDocument(reader, XmlSpace.Preserve);

                // Create an XmlTextWriter
                var ms = new MemoryStream();
                XmlWriter writer = new XmlTextWriter(ms, null);

                // Transform the data and send the output to the memory stream
                xslt.Transform(inputData, null, writer);
                ms.Position = 0;
                return ms;
            }
        }

        public override Object GetOutput(Type type) {
            if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));
            return (Stream) GetOutput();
        }
    }


    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDsigEnvelopedSignatureTransform : Transform {
        private XmlNodeList _inputNodeList;
        private Boolean _includeComments;
        private XmlNamespaceManager _nsm;
        private XmlDocument _containingDocument;
        private Int32 _signaturePosition;

        internal Int32 SignaturePosition {
            set { _signaturePosition = value; }
        }

        public XmlDsigEnvelopedSignatureTransform() {
            Algorithm = SignedXml.XmlDsigEnvelopedSignatureTransformUrl;
        }

        /// <internalonly/>
        public XmlDsigEnvelopedSignatureTransform(Boolean includeComments) {
            _includeComments = includeComments;
            Algorithm = SignedXml.XmlDsigEnvelopedSignatureTransformUrl;
        }

        public override Type[] InputTypes { get; } = { typeof(Stream), typeof(XmlNodeList), typeof(XmlDocument) };

        public override Type[] OutputTypes { get; } = { typeof(XmlNodeList), typeof(XmlDocument) };

        // An enveloped signature has no inner XML elements
        public override void LoadInnerXml(XmlNodeList nodeList) {
            if (!Utils.GetAllowAdditionalSignatureNodes() && nodeList != null && nodeList.Count > 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
        }

        // An enveloped signature has no inner XML elements
        protected override XmlNodeList GetInnerXml() {
            return null;
        }

        public override void LoadInput(Object obj) {
            if (obj is Stream) {
                LoadStreamInput((Stream) obj);
                return;
            }
            if (obj is XmlNodeList) {
                LoadXmlNodeListInput((XmlNodeList) obj);
                return;
            }
            if (obj is XmlDocument) {
                LoadXmlDocumentInput((XmlDocument) obj);
                return;
            }
        }

        private void LoadStreamInput(Stream stream) {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            var resolver = (ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), BaseURI));
            var xmlReader = Utils.PreProcessStreamInput(stream, resolver, BaseURI);
            doc.Load(xmlReader);
            _containingDocument = doc;
            if (_containingDocument == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_EnvelopedSignatureRequiresContext"));
            _nsm = new XmlNamespaceManager(_containingDocument.NameTable);
            _nsm.AddNamespace("dsig", SignedXml.XmlDsigNamespaceUrl);
        }

        private void LoadXmlNodeListInput(XmlNodeList nodeList) {
            // Empty node list is not acceptable
            if (nodeList == null) 
                throw new ArgumentNullException(nameof(nodeList));
            _containingDocument = Utils.GetOwnerDocument(nodeList);
            if (_containingDocument == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_EnvelopedSignatureRequiresContext"));

            _nsm = new XmlNamespaceManager(_containingDocument.NameTable);
            _nsm.AddNamespace("dsig", SignedXml.XmlDsigNamespaceUrl);
            _inputNodeList = nodeList;
        }

        private void LoadXmlDocumentInput(XmlDocument doc) {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));
            _containingDocument = doc;
            _nsm = new XmlNamespaceManager(_containingDocument.NameTable);
            _nsm.AddNamespace("dsig", SignedXml.XmlDsigNamespaceUrl);   
        }

        public override Object GetOutput() {
            if (_containingDocument == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_EnvelopedSignatureRequiresContext"));

            // If we have received an XmlNodeList as input
            if (_inputNodeList != null) {
                // If the position has not been set, then we don't want to remove any signature tags
                if (_signaturePosition == 0) return _inputNodeList;
                var signatureList = _containingDocument.SelectNodes("//dsig:Signature", _nsm);
                if (signatureList == null) return _inputNodeList;

                var resultNodeList = new CanonicalXmlNodeList();
                foreach (XmlNode node in _inputNodeList) {
                    if (node == null)  continue;
                    // keep namespaces
                    if (Utils.IsXmlNamespaceNode(node) || Utils.IsNamespaceNode(node)) {
                        resultNodeList.Add(node);
                    } else {
                        // SelectSingleNode throws an exception for xmldecl PI for example, so we will just ignore those exceptions
                        try {
                            // Find the nearest signature ancestor tag 
                            var result = node.SelectSingleNode("ancestor-or-self::dsig:Signature[1]", _nsm);
                            var position = 0;
                            foreach (XmlNode node1 in signatureList) {
                                position++;
                                if (node1 == result) break;
                            } 
                            if (result == null || (result != null && position != _signaturePosition)) {
                                resultNodeList.Add(node);
                            }
                        }
                        catch {}
                    }
                }
                return resultNodeList;
            }
            // Else we have received either a stream or a document as input
            else {
                var signatureList = _containingDocument.SelectNodes("//dsig:Signature", _nsm);
                if (signatureList == null) return _containingDocument;
                if (signatureList.Count < _signaturePosition || _signaturePosition <= 0) return _containingDocument;

                // Remove the signature node with all its children nodes
                signatureList[_signaturePosition - 1].ParentNode.RemoveChild(signatureList[_signaturePosition - 1]);
                return _containingDocument;
            }
        }

        public override Object GetOutput(Type type) {
            if (type == typeof(XmlNodeList) || type.IsSubclassOf(typeof(XmlNodeList))) {
                if (_inputNodeList == null) {
                    _inputNodeList = Utils.AllDescendantNodes(_containingDocument, true);
                }
                return (XmlNodeList) GetOutput();
            } else if (type == typeof(XmlDocument) || type.IsSubclassOf(typeof(XmlDocument))) {
                if (_inputNodeList != null) throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));
                return (XmlDocument) GetOutput();
            } else {
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));            
            }
        }
    }

    [Serializable]
    internal enum TransformInputType {
        XmlDocument = 1,
        XmlStream   = 2,
        XmlNodeSet  = 3
    }

    // XML Decryption Transform is used to specify the order of XML Digital Signature 
    // and XML Encryption when performed on the same document.

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlDecryptionTransform : Transform {
        private XmlNodeList m_encryptedDataList;
        private ArrayList m_arrayListUri; // this ArrayList object represents the Uri's to be excluded
        private EncryptedXml m_exml; // defines the XML encryption processing rules
        private XmlDocument m_containingDocument;
        private XmlNamespaceManager m_nsm;
        private const String XmlDecryptionTransformNamespaceUrl = "http://www.w3.org/2002/07/decrypt#";

        public XmlDecryptionTransform() {
            Algorithm = SignedXml.XmlDecryptionTransformUrl;
        }

        private ArrayList ExceptUris {
            get {
                if (m_arrayListUri == null)
                    m_arrayListUri = new ArrayList();
                return m_arrayListUri;
            }
        }

        protected virtual Boolean IsTargetElement (XmlElement inputElement, String idValue) {
            if (inputElement == null)
                return false;
            if (inputElement.GetAttribute("Id") == idValue || inputElement.GetAttribute("id") == idValue ||
                inputElement.GetAttribute("ID") == idValue)
                return true;

            return false;
        }

        public EncryptedXml EncryptedXml {
            get {
                if (m_exml != null)
                    return m_exml;

                var reference = Reference;
                var signedXml = (reference == null ? SignedXml : reference.SignedXml);
                if (signedXml == null || signedXml.EncryptedXml == null)
                    m_exml = new EncryptedXml(m_containingDocument); // default processing rules
                else
                    m_exml = signedXml.EncryptedXml;

                return m_exml;
            }
            set { m_exml = value; }
        }

        public override Type[] InputTypes { get; } = { typeof(Stream), typeof(XmlDocument) };

        public override Type[] OutputTypes { get; } = { typeof(XmlDocument) };

        public void AddExceptUri (String uri) {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            ExceptUris.Add(uri);
        }

        public override void LoadInnerXml(XmlNodeList nodeList) {
            if (nodeList == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
            ExceptUris.Clear();
            foreach (XmlNode node in nodeList) {
                var elem = node as XmlElement;
                if (elem != null) {
                    if (elem.LocalName == "Except" && elem.NamespaceURI == XmlDecryptionTransformNamespaceUrl) {
                        // the Uri is required
                        var uri = Utils.GetAttribute(elem, "URI", XmlDecryptionTransformNamespaceUrl);
                        if (uri == null || uri.Length == 0 || uri[0] != '#') 
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UriRequired"));
                        if (!Utils.VerifyAttributes(elem, "URI")) {
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                        }
                        var idref = Utils.ExtractIdFromLocalUri(uri);
                        ExceptUris.Add(idref);
                    }
                    else if (!Utils.GetAllowAdditionalSignatureNodes()) {
                        throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
                    }
                }
            }
        }

        protected override XmlNodeList GetInnerXml() {
            if (ExceptUris.Count == 0)
                return null;
            var document = new XmlDocument();
            var element = document.CreateElement("Transform", SignedXml.XmlDsigNamespaceUrl);
            if (!String.IsNullOrEmpty(Algorithm))
                element.SetAttribute("Algorithm", Algorithm);
            foreach (String uri in ExceptUris) {
                var exceptUriElement = document.CreateElement("Except", XmlDecryptionTransformNamespaceUrl);
                exceptUriElement.SetAttribute("URI", uri);
                element.AppendChild(exceptUriElement);
            }
            return element.ChildNodes;
        }

        public override void LoadInput(Object obj) {
            if (obj is Stream) {
                LoadStreamInput((Stream) obj);
            } else if (obj is XmlDocument) {
                LoadXmlDocumentInput((XmlDocument) obj);
            }
        }

        private void LoadStreamInput(Stream stream) {
            var document = new XmlDocument();
            document.PreserveWhitespace = true;
            var resolver = (ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), BaseURI));
            var xmlReader = Utils.PreProcessStreamInput(stream, resolver, BaseURI);
            document.Load(xmlReader);
            m_containingDocument = document;
            m_nsm = new XmlNamespaceManager(m_containingDocument.NameTable);
            m_nsm.AddNamespace("enc", EncryptedXml.XmlEncNamespaceUrl);
            // select all EncryptedData elements
            m_encryptedDataList = document.SelectNodes("//enc:EncryptedData", m_nsm);
        }

        private void LoadXmlDocumentInput(XmlDocument document) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            m_containingDocument = document;
            m_nsm = new XmlNamespaceManager(document.NameTable);
            m_nsm.AddNamespace("enc", EncryptedXml.XmlEncNamespaceUrl);
            // select all EncryptedData elements
            m_encryptedDataList = document.SelectNodes("//enc:EncryptedData", m_nsm);
        }

        // Replace the encrytped XML element with the decrypted data for signature verification
        private void ReplaceEncryptedData(XmlElement encryptedDataElement, Byte[] decrypted) {
            var parent = encryptedDataElement.ParentNode;
            if (parent.NodeType == XmlNodeType.Document) {
                // We're replacing the root element.  In order to correctly reflect the semantics of the
                // decryption transform, we need to replace the entire document with the decrypted data. 
                // However, EncryptedXml.ReplaceData will preserve other top-level elements such as the XML
                // entity declaration and top level comments.  So, in this case we must do the replacement
                // ourselves.
                parent.InnerXml = EncryptedXml.Encoding.GetString(decrypted);
            }
            else {
                // We're replacing a node in the middle of the document - EncryptedXml knows how to handle
                // this case in conformance with the transform's requirements, so we'll just defer to it.
                EncryptedXml.ReplaceData(encryptedDataElement, decrypted);
            }
        }

        private Boolean ProcessEncryptedDataItem (XmlElement encryptedDataElement) {
            // first see whether we want to ignore this one
            if (ExceptUris.Count > 0) {
                for (var index = 0; index < ExceptUris.Count; index++) {
                    if (IsTargetElement(encryptedDataElement, (String) ExceptUris[index]))
                        return false;
                }
            }
            var ed = new EncryptedData();
            ed.LoadXml(encryptedDataElement);
            var symAlg = EncryptedXml.GetDecryptionKey(ed, null);
            if (symAlg == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_MissingDecryptionKey"));
            var decrypted = EncryptedXml.DecryptData(ed, symAlg);

            ReplaceEncryptedData(encryptedDataElement, decrypted);
            return true;
        }

        private void ProcessElementRecursively (XmlNodeList encryptedDatas) {
            if (encryptedDatas == null || encryptedDatas.Count == 0)
                return;
            var encryptedDatasQueue = new Queue();
            foreach (XmlNode value in encryptedDatas) {
                encryptedDatasQueue.Enqueue(value);
            }
            var node = encryptedDatasQueue.Dequeue() as XmlNode;
            while (node != null) {
                var encryptedDataElement = node as XmlElement;
                if (encryptedDataElement != null && encryptedDataElement.LocalName == "EncryptedData" &&
                    encryptedDataElement.NamespaceURI == EncryptedXml.XmlEncNamespaceUrl) {
                    var sibling = encryptedDataElement.NextSibling;
                    var parent = encryptedDataElement.ParentNode;
                    if (ProcessEncryptedDataItem(encryptedDataElement)) {
                        // find the new decrypted element.
                        var child = parent.FirstChild;
                        while (child != null && child.NextSibling != sibling)
                            child = child.NextSibling;
                        if (child != null) {
                            var nodes = child.SelectNodes("//enc:EncryptedData", m_nsm);
                            if (nodes.Count > 0) {
                                foreach (XmlNode value in nodes) {
                                    encryptedDatasQueue.Enqueue(value);
                                }
                            }
                        }
                    }
                }
                if (encryptedDatasQueue.Count == 0)
                    break;
                node = encryptedDatasQueue.Dequeue() as XmlNode;
            }
        }

        public override Object GetOutput() {
            // decrypt the encrypted sections
            if (m_encryptedDataList != null)
                ProcessElementRecursively(m_encryptedDataList);
            // propagate namespaces
            Utils.AddNamespaces(m_containingDocument.DocumentElement, PropagatedNamespaces);
            return m_containingDocument;
        }

        public override Object GetOutput(Type type) {
            if (type == typeof(XmlDocument))
                return (XmlDocument) GetOutput();
            else
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));
        }
    }

    [System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public class XmlLicenseTransform : Transform {
        private XmlNamespaceManager namespaceManager;
        private XmlDocument         license;
        private const String        ElementIssuer    = "issuer";
        private const String        NamespaceUriCore = "urn:mpeg:mpeg21:2003:01-REL-R-NS";

        public XmlLicenseTransform() {
            Algorithm = SignedXml.XmlLicenseTransformUrl;
        }

        public override Type[] InputTypes { get; } = { typeof(XmlDocument) };

        public override Type[] OutputTypes { get; } = { typeof(XmlDocument) };

        public IRelDecryptor Decryptor { get; set; }

        private void DecryptEncryptedGrants(XmlNodeList encryptedGrantList, IRelDecryptor decryptor) {
            XmlElement       encryptionMethod    = null;
            XmlElement       keyInfo             = null;
            XmlElement       cipherData          = null;
            EncryptionMethod encryptionMethodObj = null;
            KeyInfo          keyInfoObj          = null;
            CipherData       cipherDataObj       = null;

            for (Int32 i = 0, count = encryptedGrantList.Count; i < count; i++) {
                encryptionMethod = encryptedGrantList[i].SelectSingleNode("//r:encryptedGrant/enc:EncryptionMethod", namespaceManager) as XmlElement;
                keyInfo          = encryptedGrantList[i].SelectSingleNode("//r:encryptedGrant/dsig:KeyInfo", namespaceManager) as XmlElement;
                cipherData       = encryptedGrantList[i].SelectSingleNode("//r:encryptedGrant/enc:CipherData", namespaceManager) as XmlElement;
                if ((encryptionMethod != null) &&
                    (keyInfo != null) &&
                    (cipherData != null)) {
                    encryptionMethodObj = new EncryptionMethod();
                    keyInfoObj          = new KeyInfo();
                    cipherDataObj       = new CipherData();

                    encryptionMethodObj.LoadXml(encryptionMethod);
                    keyInfoObj.LoadXml(keyInfo);
                    cipherDataObj.LoadXml(cipherData);

                    MemoryStream toDecrypt        = null;
                    Stream       decryptedContent = null;
                    StreamReader streamReader     = null;

                    try {
                        toDecrypt = new MemoryStream(cipherDataObj.CipherValue);
                        decryptedContent = Decryptor.Decrypt(encryptionMethodObj,
                                                                keyInfoObj, toDecrypt);

                        if ((decryptedContent == null) || (decryptedContent.Length == 0))
                            throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_XrmlUnableToDecryptGrant"));

                        streamReader = new StreamReader(decryptedContent);
                        var clearContent = streamReader.ReadToEnd();

                        encryptedGrantList[i].ParentNode.InnerXml = clearContent;
                    }
                    finally {
                        if (toDecrypt != null)
                            toDecrypt.Close();

                        if (decryptedContent != null)
                            decryptedContent.Close();

                        if (streamReader != null)
                            streamReader.Close();
                    }

                    encryptionMethodObj = null;
                    keyInfoObj          = null;
                    cipherDataObj       = null;
                }

                encryptionMethod = null;
                keyInfo          = null;
                cipherData       = null;
            }
        }

        // License transform has no inner XML elements
        protected override XmlNodeList GetInnerXml() {
            return null;
        }

        public override Object GetOutput() {
            return license;
        }

        public override Object GetOutput(Type type) {
            if ((type != typeof(XmlDocument)) || (!type.IsSubclassOf(typeof(XmlDocument))))
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_Xml_TransformIncorrectInputType"), nameof(type));

            return GetOutput();
        }

        // License transform has no inner XML elements
        public override void LoadInnerXml(XmlNodeList nodeList) {
            if (!Utils.GetAllowAdditionalSignatureNodes() && nodeList != null && nodeList.Count > 0)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_UnknownTransform"));
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3058:DoNotUseSetInnerXml", Justification="Operates on inputs which were already parsed by XmlDocument with valid settings and already would have produced errors (DTD or external resolution)")]
        public override void LoadInput (Object obj) {
            // Check if the Context property is set before this transform is invoked.
            if (Context == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_XrmlMissingContext"));

            license = new XmlDocument();
            license.PreserveWhitespace = true;
            namespaceManager = new XmlNamespaceManager(license.NameTable);
            namespaceManager.AddNamespace("dsig", SignedXml.XmlDsigNamespaceUrl);
            namespaceManager.AddNamespace("enc", EncryptedXml.XmlEncNamespaceUrl);
            namespaceManager.AddNamespace("r", NamespaceUriCore);

            XmlElement currentIssuerContext  = null;
            XmlElement currentLicenseContext = null;
            XmlNode    signatureNode         = null;

            // Get the nearest issuer node
            currentIssuerContext = Context.SelectSingleNode("ancestor-or-self::r:issuer[1]", namespaceManager) as XmlElement;
            if (currentIssuerContext == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_XrmlMissingIssuer"));

            signatureNode = currentIssuerContext.SelectSingleNode("descendant-or-self::dsig:Signature[1]", namespaceManager) as XmlElement;
            if (signatureNode != null)
                signatureNode.ParentNode.RemoveChild(signatureNode);

            // Get the nearest license node
            currentLicenseContext = currentIssuerContext.SelectSingleNode("ancestor-or-self::r:license[1]", namespaceManager) as XmlElement;
            if (currentLicenseContext == null)
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_XrmlMissingLicence"));

            var issuerList = currentLicenseContext.SelectNodes("descendant-or-self::r:license[1]/r:issuer", namespaceManager);

            // Remove all issuer nodes except current
            for (Int32 i = 0, count = issuerList.Count; i < count; i++) {
                if (issuerList[i] == currentIssuerContext)
                    continue;

                if ((issuerList[i].LocalName == ElementIssuer) && 
                    (issuerList[i].NamespaceURI == NamespaceUriCore))
                    issuerList[i].ParentNode.RemoveChild(issuerList[i]);
            }

            var encryptedGrantList = currentLicenseContext.SelectNodes("/r:license/r:grant/r:encryptedGrant", namespaceManager);

            if (encryptedGrantList.Count > 0) {
                if (Decryptor == null)
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_XrmlMissingIRelDecryptor"));

                DecryptEncryptedGrants(encryptedGrantList, Decryptor);
            }

            license.InnerXml = currentLicenseContext.OuterXml;
        }
    }

    public interface IRelDecryptor {
        Stream Decrypt(EncryptionMethod encryptionMethod, KeyInfo keyInfo, Stream toDecrypt);
    }
}
