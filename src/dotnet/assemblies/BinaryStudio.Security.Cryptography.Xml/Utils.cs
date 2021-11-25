using System;
using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml;
using BinaryStudio.Security.Cryptography.Xml.Properties;

namespace BinaryStudio.Security.Cryptography.Xml
{
    [Serializable]
    internal enum CertUsageType {
        Verification = 0,
        Decryption   = 1
    }

    internal class MyXmlDocument : XmlDocument {
        protected override XmlAttribute CreateDefaultAttribute(String prefix, String localName, String namespaceURI) {
            return CreateAttribute(prefix, localName, namespaceURI);
        }
    }

    internal class Utils {
        private Utils () {}

        private static Boolean HasNamespace (XmlElement element, String prefix, String value) {
            if (IsCommittedNamespace(element, prefix, value)) return true;
            if (element.Prefix == prefix && element.NamespaceURI == value) return true;
            return false;
        }

        // A helper function that determines if a namespace node is a committed attribute
        internal static Boolean IsCommittedNamespace (XmlElement element, String prefix, String value) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var name = ((prefix.Length > 0) ? "xmlns:" + prefix : "xmlns");
            if (element.HasAttribute(name) && element.GetAttribute(name) == value) return true;
            return false;
        }

        internal static Boolean IsRedundantNamespace (XmlElement element, String prefix, String value) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var ancestorNode = ((XmlNode)element).ParentNode;
            while (ancestorNode != null) {
                var ancestorElement = ancestorNode as XmlElement;
                if (ancestorElement != null) 
                    if (HasNamespace(ancestorElement, prefix, value)) return true;
                ancestorNode = ancestorNode.ParentNode;
            }

            return false;
        }

        internal static String GetAttribute (XmlElement element, String localName, String namespaceURI) {
            var s = (element.HasAttribute(localName) ? element.GetAttribute(localName) : null);
            if (s == null && element.HasAttribute(localName, namespaceURI))
                s = element.GetAttribute(localName, namespaceURI);
            return s;
        }

        internal static Boolean HasAttribute (XmlElement element, String localName, String namespaceURI) {
            return element.HasAttribute(localName) || element.HasAttribute(localName, namespaceURI);
        }

        internal static Boolean IsNamespaceNode (XmlNode n) {
            return n.NodeType == XmlNodeType.Attribute && (n.Prefix.Equals("xmlns") || (n.Prefix.Length == 0 && n.LocalName.Equals("xmlns")));
        }

        internal static Boolean IsXmlNamespaceNode (XmlNode n) {
            return n.NodeType == XmlNodeType.Attribute && n.Prefix.Equals("xml");
        }

        // We consider xml:space style attributes as default namespace nodes since they obey the same propagation rules
        internal static Boolean IsDefaultNamespaceNode (XmlNode n) {
            var b1 = n.NodeType == XmlNodeType.Attribute && n.Prefix.Length == 0 && n.LocalName.Equals("xmlns");
            var b2 = IsXmlNamespaceNode(n);
            return b1 || b2;
        }

        internal static Boolean IsEmptyDefaultNamespaceNode (XmlNode n) {
            return IsDefaultNamespaceNode(n) && n.Value.Length == 0;
        }

        internal static String GetNamespacePrefix (XmlAttribute a) {
            Debug.Assert(IsNamespaceNode(a) || IsXmlNamespaceNode(a));
            return a.Prefix.Length == 0 ? String.Empty : a.LocalName;
        }

        internal static Boolean HasNamespacePrefix (XmlAttribute a, String nsPrefix) {
            return GetNamespacePrefix(a).Equals(nsPrefix);
        }

        internal static Boolean IsNonRedundantNamespaceDecl (XmlAttribute a, XmlAttribute nearestAncestorWithSamePrefix) {
            if (nearestAncestorWithSamePrefix == null)
                return !IsEmptyDefaultNamespaceNode(a);
            else
                return !nearestAncestorWithSamePrefix.Value.Equals(a.Value);
        }

        internal static Boolean IsXmlPrefixDefinitionNode (XmlAttribute a) {
            return false;
//            return a.Prefix.Equals("xmlns") && a.LocalName.Equals("xml") && a.Value.Equals(NamespaceUrlForXmlPrefix);
        }

        internal static String DiscardWhiteSpaces (String inputBuffer) {
            return DiscardWhiteSpaces(inputBuffer, 0, inputBuffer.Length);
        }


        internal static String DiscardWhiteSpaces (String inputBuffer, Int32 inputOffset, Int32 inputCount) {
            Int32 i, iCount = 0;
            for (i=0; i<inputCount; i++)
                if (Char.IsWhiteSpace(inputBuffer[inputOffset + i])) iCount++;
            var rgbOut = new Char[inputCount - iCount];
            iCount = 0;
            for (i=0; i<inputCount; i++)
                if (!Char.IsWhiteSpace(inputBuffer[inputOffset + i])) {
                    rgbOut[iCount++] = inputBuffer[inputOffset + i];
                }
            return new String(rgbOut);
        }

        internal static void SBReplaceCharWithString (StringBuilder sb, Char oldChar, String newString) {
            var i = 0;
            var newStringLength = newString.Length;
            while (i < sb.Length) {
                if (sb[i] == oldChar) {
                    sb.Remove(i,1);
                    sb.Insert(i,newString);
                    i += newStringLength;
                } else i++;
            }
        }

        internal static XmlReader PreProcessStreamInput (Stream inputStream, XmlResolver xmlResolver, String baseUri) {
            var settings = GetSecureXmlReaderSettings(xmlResolver);
            var reader = XmlReader.Create(inputStream, settings, baseUri);
            return reader;
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3069:ReviewDtdProcessingAssignment", Justification= "DTD risks are mitigated by URI restrictions and expansion limits")]
        internal static XmlReaderSettings GetSecureXmlReaderSettings(XmlResolver xmlResolver)
        {
        var settings = new XmlReaderSettings
            {
            XmlResolver = xmlResolver,
            DtdProcessing = DtdProcessing.Parse,
            MaxCharactersFromEntities = GetMaxCharactersFromEntities(),
            MaxCharactersInDocument = GetMaxCharactersInDocument()
            };
        return settings;
        }

        private static Int32? xmlDsigSearchDepth;
        /// <summary>
        /// Function get the XML Dsig recursion limit. This function defines the
        /// default limit in case, limit is not defined by developer or admin then
        /// it returns the default value.
        /// </summary>
        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Int32 GetXmlDsigSearchDepth() {
            if (xmlDsigSearchDepth.HasValue) {
                return xmlDsigSearchDepth.Value;
            }
            //Keeping the default recursion limit to 20. It should be
            //within limits of real world scenarios. Keeping this number low
            //will preserve some stack space
            var maxXmlDsigSearchDepth = GetNetFxSecurityRegistryValue("SignedDigitalSignatureXmlMaxDepth", 20);

            xmlDsigSearchDepth = (Int32)maxXmlDsigSearchDepth;
            return xmlDsigSearchDepth.Value;
        }

        private static Int64? maxCharactersFromEntities;
        // Allow machine admins to specify an entity expansion limit. This is used to prevent
        // entity expansion denial of service attacks.
        // Falls back to a default if none is specified.
        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Int64 GetMaxCharactersFromEntities() {
            if (maxCharactersFromEntities.HasValue) {
                return maxCharactersFromEntities.Value;
            }

            var maxCharacters = GetNetFxSecurityRegistryValue("SignedXmlMaxCharactersFromEntities", (Int64)1e7);

            maxCharactersFromEntities = maxCharacters;
            return maxCharactersFromEntities.Value;
        }

        private static Boolean s_readMaxCharactersInDocument;
        private static Int64 s_maxCharactersInDocument;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Int64 GetMaxCharactersInDocument()
        {
            // Allow machine administrators to specify a maximum document load size for SignedXml.
            if (s_readMaxCharactersInDocument)
            {
                return s_maxCharactersInDocument;
            }

            // The default value, 0, is "no limit"
            var maxCharacters = GetNetFxSecurityRegistryValue("SignedXmlMaxCharactersInDocument", 0);

            s_maxCharactersInDocument = maxCharacters;
            Thread.MemoryBarrier();
            s_readMaxCharactersInDocument = true;

            return s_maxCharactersInDocument;
        }

        private static Boolean? s_allowAmbiguousReferenceTarget;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Boolean AllowAmbiguousReferenceTargets()
        {
            // Allow machine administrators to specify that the legacy behavior of matching the first element
            // in an ambiguous reference situation should be persisted. The default behavior is to throw in that
            // situation, but a REG_DWORD or REG_QWORD value of 1 will revert.
            if (s_allowAmbiguousReferenceTarget.HasValue)
            {
                return s_allowAmbiguousReferenceTarget.Value;
            }

            var numericValue = GetNetFxSecurityRegistryValue("SignedXmlAllowAmbiguousReferenceTargets", 0);
            var allowAmbiguousReferenceTarget = numericValue != 0;

            s_allowAmbiguousReferenceTarget = allowAmbiguousReferenceTarget;
            return s_allowAmbiguousReferenceTarget.Value;
        }

        private static Boolean? s_allowDetachedSignature;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Boolean AllowDetachedSignature()
        {
            // Allow machine administrators to specify that detached signatures can be processed.
            // The default behavior is to throw when processing a detached signature,
            // but a REG_DWORD or REG_QWORD value of 1 will revert.
            if (s_allowDetachedSignature.HasValue)
            {
                return s_allowDetachedSignature.Value;
            }

            var numericValue = GetNetFxSecurityRegistryValue("SignedXmlAllowDetachedSignature", 0);
            var allowDetachedSignature = numericValue != 0;

            s_allowDetachedSignature = allowDetachedSignature;
            return s_allowDetachedSignature.Value;
        }

        private static Boolean s_readRequireNCNameIdentifier;
        private static Boolean s_requireNCNameIdentifier = true;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Boolean RequireNCNameIdentifier()
        {
            if (s_readRequireNCNameIdentifier)
            {
                return s_requireNCNameIdentifier;
            }

            var numericValue = GetNetFxSecurityRegistryValue("SignedXmlRequireNCNameIdentifier", 1);
            var requireNCName = numericValue != 0;

            s_requireNCNameIdentifier = requireNCName;
            Thread.MemoryBarrier();
            s_readRequireNCNameIdentifier = true;

            return s_requireNCNameIdentifier;
        }

        private static Boolean s_readMaxTransformsPerReference;
        private static Int64 s_maxTransformsPerReference = 10;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Int64 GetMaxTransformsPerReference()
        {
            // Allow machine administrators to specify a maximum number of Transforms per Reference in SignedXML.
            if (s_readMaxTransformsPerReference)
            {
                return s_maxTransformsPerReference;
            }

            var maxTransforms = GetNetFxSecurityRegistryValue("SignedXmlMaxTransformsPerReference", 10);

            s_maxTransformsPerReference = maxTransforms;
            Thread.MemoryBarrier();
            s_readMaxTransformsPerReference = true;

            return s_maxTransformsPerReference;
        }

        private static Boolean s_readMaxReferencesPerSignedInfo;
        private static Int64 s_maxReferencesPerSignedInfo = 100;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Int64 GetMaxReferencesPerSignedInfo()
        {
            // Allow machine administrators to specify a maximum number of References per SignedInfo/Signature in SignedXML.
            if (s_readMaxReferencesPerSignedInfo)
            {
                return s_maxReferencesPerSignedInfo;
            }

            var maxReferences = GetNetFxSecurityRegistryValue("SignedXmlMaxReferencesPerSignedInfo", 100);

            s_maxReferencesPerSignedInfo = maxReferences;
            Thread.MemoryBarrier();
            s_readMaxReferencesPerSignedInfo = true;

            return s_maxReferencesPerSignedInfo;
        }

        private static Boolean s_readAllowAdditionalSignatureNodes;
        private static Boolean s_allowAdditionalSignatureNodes;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Boolean GetAllowAdditionalSignatureNodes()
        {
            // Allow machine administrators to specify whether SignedXML should allow Signature nodes that don't conform to the spec.
            if (s_readAllowAdditionalSignatureNodes)
            {
                return s_allowAdditionalSignatureNodes;
            }

            var numericValue = GetNetFxSecurityRegistryValue("SignedXmlAllowAdditionalSignatureNodes", 0);
            var allowAdditionalSignatureNodes = numericValue != 0;

            s_allowAdditionalSignatureNodes = allowAdditionalSignatureNodes;
            Thread.MemoryBarrier();
            s_readAllowAdditionalSignatureNodes = true;

            return s_allowAdditionalSignatureNodes;
        }

        private static Boolean s_readSkipSignatureAttributeEnforcement;
        private static Boolean s_skipSignatureAttributeEnforcement;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Boolean GetSkipSignatureAttributeEnforcement()
        {
            // Allow machine administrators to specify whether SignedXML should skip enforcement of
            // spec Attribute rules in Signature nodes.
            if (s_readSkipSignatureAttributeEnforcement)
            {
                return s_skipSignatureAttributeEnforcement;
            }

            var numericValue = GetNetFxSecurityRegistryValue("SignedXmlSkipSignatureAttributeEnforcement", 0);
            var skipSignatureAttributeEnforcement = numericValue != 0;

            s_skipSignatureAttributeEnforcement = skipSignatureAttributeEnforcement;
            Thread.MemoryBarrier();
            s_readSkipSignatureAttributeEnforcement = true;

            return s_skipSignatureAttributeEnforcement;
        }

        private static Boolean s_readAllowBareTypeReference;
        private static Boolean s_allowBareTypeReference;

        internal static Boolean VerifyAttributes(XmlElement element, String expectedAttrName)
        {
            return VerifyAttributes(element, expectedAttrName == null ? null : new String[] { expectedAttrName });
        }

        internal static Boolean VerifyAttributes(XmlElement element, String[] expectedAttrNames)
        {
            if (!GetSkipSignatureAttributeEnforcement())
            {
                foreach (XmlAttribute attr in element.Attributes)
                {
                    // There are a few Xml Special Attributes that are always allowed on any node. Make sure we allow those here.
                    var attrIsAllowed = attr.Name == "xmlns" || attr.Name.StartsWith("xmlns:") || attr.Name == "xml:space" || attr.Name == "xml:lang" || attr.Name == "xml:base";
                    var expectedInd = 0;
                    while (!attrIsAllowed && expectedAttrNames != null && expectedInd < expectedAttrNames.Length)
                    {
                        attrIsAllowed = attr.Name == expectedAttrNames[expectedInd];
                        expectedInd++;
                    }
                    if (!attrIsAllowed)
                        return false;
                }
            }
            return true;
        }

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Boolean GetAllowBareTypeReference()
        {
            if (s_readAllowBareTypeReference)
            {
                return s_allowBareTypeReference;
            }

            var numericValue = GetNetFxSecurityRegistryValue("CryptoXmlAllowBareTypeReference", 0);
            var allowBareReference = numericValue != 0;

            s_allowBareTypeReference = allowBareReference;
            Thread.MemoryBarrier();
            s_readAllowBareTypeReference = true;

            return s_allowBareTypeReference;
        }

        private static Boolean s_readLeaveCipherValueUnchecked;
        private static Boolean s_leaveCipherValueUnchecked;

        [RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        internal static Boolean GetLeaveCipherValueUnchecked()
        {
            if (s_readLeaveCipherValueUnchecked)
            {
                return s_leaveCipherValueUnchecked;
            }

            var numericValue = GetNetFxSecurityRegistryValue("EncryptedXmlLeaveCipherValueUnchecked", 0);
            var leaveCipherValueUnchecked = numericValue != 0;

            s_leaveCipherValueUnchecked = leaveCipherValueUnchecked;
            Thread.MemoryBarrier();
            s_readLeaveCipherValueUnchecked = true;

            return s_leaveCipherValueUnchecked;
        }

        private static readonly Char[] s_invalidChars = new Char[] { ',', '`', '[', '*', '&' };

        public static Object CreateFromKnownName(String name)
            {
            switch (name)
                {
                case "http://www.w3.org/TR/2001/REC-xml-c14n-20010315":              { return new XmlDsigC14NTransform();                }
                case "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments": { return new XmlDsigC14NWithCommentsTransform();    }
                case "http://www.w3.org/2001/10/xml-exc-c14n#":                      { return new XmlDsigExcC14NTransform();             }
                case "http://www.w3.org/2001/10/xml-exc-c14n#WithComments":          { return new XmlDsigExcC14NWithCommentsTransform(); }
                case "http://www.w3.org/2000/09/xmldsig#base64":                     { return new XmlDsigBase64Transform();              }
                case "http://www.w3.org/TR/1999/REC-xpath-19991116":                 { return new XmlDsigXPathTransform();               }
                case "http://www.w3.org/TR/1999/REC-xslt-19991116":                  { return new XmlDsigXsltTransform();                }
                case "http://www.w3.org/2000/09/xmldsig#enveloped-signature":        { return new XmlDsigEnvelopedSignatureTransform();  }
                case "http://www.w3.org/2000/09/xmldsig# X509Data":                  { return new KeyInfoX509Data();                     }
                case "http://www.w3.org/2000/09/xmldsig# KeyName":                   { return new KeyInfoName();                         }
                case "http://www.w3.org/2000/09/xmldsig# RetrievalMethod":           { return new KeyInfoRetrievalMethod();              }
                //case "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr3411":
                //case "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102001-gostr3411":
                //    return GostHashAlgorithm.Create("1.2.643.2.2.9");
                //case "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-256":
                //case "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-256":
                //    return GostHashAlgorithm.Create("1.2.643.7.1.1.2.2");
                //case "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-512":
                //case "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-512":
                //    return GostHashAlgorithm.Create("1.2.643.7.1.1.2.3");
                }
            return null;
            }
        internal static T CreateFromName<T>(String key) where T : class
            {
            if (GetAllowBareTypeReference())
                {
                return CryptoConfig.CreateFromName(key) as T;
                }

            if (key == null || key.IndexOfAny(s_invalidChars) >= 0)
                {
                return null;
                }

            try
                {
                var r = (CreateFromKnownName(key) ?? CryptoConfig.CreateFromName(key));
                return r as T;
                }
            catch (Exception)
                {
                return null;
                }
            }

        private static Int64 GetNetFxSecurityRegistryValue(String regValueName, Int64 defaultValue)
        {
            try {
                using (var securityRegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Security", false)) {
                    if (securityRegKey != null) {
                        var regValue = securityRegKey.GetValue(regValueName);
                        if (regValue != null) {
                            var valueKind = securityRegKey.GetValueKind(regValueName);
                            if (valueKind == RegistryValueKind.DWord || valueKind == RegistryValueKind.QWord) {
                                return Convert.ToInt64(regValue, CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
            }
            catch (SecurityException) { /* we could not open the key - that's fine, we can proceed with the default value */ }

            return defaultValue;
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3069:ReviewDtdProcessingAssignment", Justification= "Required for re-parsing documents which were user-loaded with DtdProcessing.Parse")]
        internal static XmlDocument PreProcessDocumentInput (XmlDocument document, XmlResolver xmlResolver, String baseUri) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var doc = new MyXmlDocument();
            doc.PreserveWhitespace = document.PreserveWhitespace;

            // Normalize the document
            using (TextReader stringReader = new StringReader(document.OuterXml)) {
                var settings = new XmlReaderSettings
                    {
                    XmlResolver = xmlResolver,
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = GetMaxCharactersFromEntities(),
                    MaxCharactersInDocument = GetMaxCharactersInDocument()
                    };
                var reader = XmlReader.Create(stringReader, settings, baseUri);
                doc.Load(reader);
            }
            return doc;
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3069:ReviewDtdProcessingAssignment", Justification= "Required for re-parsing elements which were user-loaded with DtdProcessing.Parse")]
        internal static XmlDocument PreProcessElementInput (XmlElement elem, XmlResolver xmlResolver, String baseUri) {
            if (elem == null)
                throw new ArgumentNullException(nameof(elem));

            var doc = new MyXmlDocument {PreserveWhitespace = true};
            // Normalize the document
            using (TextReader stringReader = new StringReader(elem.OuterXml)) {
                var settings = new XmlReaderSettings();
                settings.XmlResolver = xmlResolver;
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.MaxCharactersFromEntities = GetMaxCharactersFromEntities();
                settings.MaxCharactersInDocument = GetMaxCharactersInDocument();
                var reader = XmlReader.Create(stringReader, settings, baseUri);
                doc.Load(reader);
            }
            return doc;
        }

        internal static XmlDocument DiscardComments (XmlDocument document) {
            var nodeList = document.SelectNodes("//comment()");
            if (nodeList != null) {
                foreach (XmlNode node1 in nodeList) {
                    node1.ParentNode.RemoveChild(node1);
                }
            }
            return document;
        }

        internal static XmlNodeList AllDescendantNodes (XmlNode node, Boolean includeComments) {
            var nodeList = new CanonicalXmlNodeList();
            var elementList = new CanonicalXmlNodeList();
            var attribList = new CanonicalXmlNodeList();
            var namespaceList = new CanonicalXmlNodeList();

            var index = 0;
            elementList.Add(node);

            do {
                var rootNode = (XmlNode) elementList[index];
                // Add the children nodes
                var childNodes = rootNode.ChildNodes;
                if (childNodes != null) {
                    foreach (XmlNode node1 in childNodes) {
                        if (includeComments || (!(node1 is XmlComment))) {
                            elementList.Add(node1);
                        }
                    }
                }
                // Add the attribute nodes
                var attribNodes = rootNode.Attributes;
                if (attribNodes != null) {
                    foreach (XmlNode attribNode in rootNode.Attributes) {
                        if (attribNode.LocalName == "xmlns" || attribNode.Prefix == "xmlns")
                            namespaceList.Add(attribNode);
                        else
                            attribList.Add(attribNode);
                    }
                }
                index++;
            } while (index < elementList.Count);
            foreach (XmlNode elementNode in elementList) {
                nodeList.Add(elementNode);
            }
            foreach (XmlNode attribNode in attribList) {
                nodeList.Add(attribNode);
            }
            foreach (XmlNode namespaceNode in namespaceList) {
                nodeList.Add(namespaceNode);
            }

            return nodeList;
        }

        internal static Boolean NodeInList (XmlNode node, XmlNodeList nodeList) {
            foreach (XmlNode nodeElem in nodeList) {
                if (nodeElem == node) return true;
            }
            return false;
        }

        internal static String GetIdFromLocalUri (String uri, out Boolean discardComments) {
            var idref = uri.Substring(1);
            // initialize the return value
            discardComments = true;

            // Deal with XPointer of type #xpointer(id("ID")). Other XPointer support isn't handled here and is anyway optional 
            if (idref.StartsWith("xpointer(id(", StringComparison.Ordinal)) {
                var startId = idref.IndexOf("id(", StringComparison.Ordinal);
                var endId = idref.IndexOf(")", StringComparison.Ordinal);
                if (endId < 0 || endId < startId + 3) 
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
                idref = idref.Substring(startId + 3, endId - startId - 3);
                idref = idref.Replace("\'", "");
                idref = idref.Replace("\"", "");
                discardComments = false;
            }
            return idref;
        }

        internal static String ExtractIdFromLocalUri (String uri) {
            var idref = uri.Substring(1);

            // Deal with XPointer of type #xpointer(id("ID")). Other XPointer support isn't handled here and is anyway optional 
            if (idref.StartsWith("xpointer(id(", StringComparison.Ordinal)) {
                var startId = idref.IndexOf("id(", StringComparison.Ordinal);
                var endId = idref.IndexOf(")", StringComparison.Ordinal);
                if (endId < 0 || endId < startId + 3) 
                    throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_Xml_InvalidReference"));
                idref = idref.Substring(startId + 3, endId - startId - 3);
                idref = idref.Replace("\'", "");
                idref = idref.Replace("\"", "");
            }
            return idref;
        }

        // This removes all children of an element.
        internal static void RemoveAllChildren (XmlElement inputElement) {
            var child = inputElement.FirstChild;
            XmlNode sibling;

            while (child != null) {
                sibling = child.NextSibling;
                inputElement.RemoveChild(child);
                child = sibling;
            }
        }

        // Writes one stream (starting from the current position) into 
        // an output stream, connecting them up and reading until 
        // hitting the end of the input stream.  
        // returns the number of bytes copied
        internal static Int64 Pump (Stream input, Stream output) { 
            // Use MemoryStream's WriteTo(Stream) method if possible
            var inputMS = input as MemoryStream;
            if (inputMS != null && inputMS.Position == 0) {
                inputMS.WriteTo(output);
                return inputMS.Length;
            }

            const Int32 count = 4096;
            var bytes = new Byte[count];
            Int32 numBytes;
            Int64 totalBytes = 0;

            while((numBytes = input.Read(bytes, 0, count)) > 0) {
                output.Write(bytes, 0, numBytes);
                totalBytes += numBytes;
            }

            return totalBytes;
        }

        internal static Hashtable TokenizePrefixListString (String s) {
            var set = new Hashtable();
            if (s != null) {
                var prefixes = s.Split(null);
                foreach (var prefix in prefixes) {
                    if (prefix.Equals("#default")) {
                        set.Add(String.Empty, true);
                    } else if (prefix.Length > 0) {
                        set.Add(prefix, true);
                    }
                }
            }
            return set;
        }

        internal static String EscapeWhitespaceData(String data) {
            var sb = new StringBuilder();
            sb.Append(data);
            SBReplaceCharWithString(sb, (Char) 13,"&#xD;");
            return sb.ToString();
        }

        internal static String EscapeTextData(String data) {
            var sb = new StringBuilder();
            sb.Append(data);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            SBReplaceCharWithString(sb, (Char) 13, "&#xD;");
            return sb.ToString();
        }

        internal static String EscapeCData(String data) {
            return EscapeTextData(data);
        }

        internal static String EscapeAttributeValue(String value) {
            var sb = new StringBuilder();
            sb.Append(value);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace("\"", "&quot;");
            SBReplaceCharWithString(sb, (Char) 9, "&#x9;");
            SBReplaceCharWithString(sb, (Char) 10, "&#xA;");
            SBReplaceCharWithString(sb, (Char) 13, "&#xD;");
            return sb.ToString();
        }

        internal static XmlDocument GetOwnerDocument (XmlNodeList nodeList) {
            foreach (XmlNode node in nodeList) {
                if (node.OwnerDocument != null)
                    return node.OwnerDocument;
            }
            return null;
        }

        internal static void AddNamespaces (XmlElement elem, CanonicalXmlNodeList namespaces) {
            if (namespaces != null) {
                foreach (XmlNode attrib in namespaces) {
                    var name = ((attrib.Prefix.Length > 0) ? attrib.Prefix + ":" + attrib.LocalName : attrib.LocalName);
                    // Skip the attribute if one with the same qualified name already exists
                    if (elem.HasAttribute(name) || (name.Equals("xmlns") && elem.Prefix.Length == 0)) continue;
                    var nsattrib = (XmlAttribute) elem.OwnerDocument.CreateAttribute(name);
                    nsattrib.Value = attrib.Value;
                    elem.SetAttributeNode(nsattrib);
                }
            }
        }

        internal static void AddNamespaces (XmlElement elem, Hashtable namespaces) {
            if (namespaces != null) {
                foreach (String key in namespaces.Keys) {
                    if (elem.HasAttribute(key)) continue;
                    var nsattrib = (XmlAttribute) elem.OwnerDocument.CreateAttribute(key);
                    nsattrib.Value = namespaces[key] as String;
                    elem.SetAttributeNode(nsattrib);
                }
            }
        }

        // This method gets the attributes that should be propagated 
        internal static CanonicalXmlNodeList GetPropagatedAttributes (XmlElement elem) {
            if (elem == null)
                return null;

            var namespaces = new CanonicalXmlNodeList();
            XmlNode ancestorNode = elem;

            if (ancestorNode == null) return null;

            var bDefNamespaceToAdd = true;

            while (ancestorNode != null) {
                var ancestorElement = ancestorNode as XmlElement;
                if (ancestorElement == null) {
                    ancestorNode = ancestorNode.ParentNode;
                    continue;
                }
                if (!IsCommittedNamespace(ancestorElement, ancestorElement.Prefix, ancestorElement.NamespaceURI)) {
                    // Add the namespace attribute to the collection if needed
                    if (!IsRedundantNamespace(ancestorElement, ancestorElement.Prefix, ancestorElement.NamespaceURI)) {
                        var name = ((ancestorElement.Prefix.Length > 0) ? "xmlns:" + ancestorElement.Prefix : "xmlns");
                        var nsattrib = elem.OwnerDocument.CreateAttribute(name);
                        nsattrib.Value = ancestorElement.NamespaceURI;
                        namespaces.Add(nsattrib);
                    }
                }
                if (ancestorElement.HasAttributes) {
                    var attribs = ancestorElement.Attributes;
                    foreach (XmlAttribute attrib in attribs) {
                        // Add a default namespace if necessary
                        if (bDefNamespaceToAdd && attrib.LocalName == "xmlns") {
                            var nsattrib = elem.OwnerDocument.CreateAttribute("xmlns");
                            nsattrib.Value = attrib.Value;
                            namespaces.Add(nsattrib);
                            bDefNamespaceToAdd = false;
                            continue;
                        }
                        // retain the declarations of type 'xml:*' as well
                        if (attrib.Prefix == "xmlns" || attrib.Prefix == "xml") {
                            namespaces.Add(attrib);
                            continue;
                        }
                        if (attrib.NamespaceURI.Length > 0) {
                            if (!IsCommittedNamespace(ancestorElement, attrib.Prefix, attrib.NamespaceURI)) {
                                // Add the namespace attribute to the collection if needed
                                if (!IsRedundantNamespace(ancestorElement, attrib.Prefix, attrib.NamespaceURI)) {
                                    var name = ((attrib.Prefix.Length > 0) ? "xmlns:" + attrib.Prefix : "xmlns");
                                    var nsattrib = elem.OwnerDocument.CreateAttribute(name);
                                    nsattrib.Value = attrib.NamespaceURI;
                                    namespaces.Add(nsattrib);
                                }
                            }
                        }
                    }
                }
                ancestorNode = ancestorNode.ParentNode;
            }

            return namespaces;
        }

        // output of this routine is always big endian
        internal static Byte[] ConvertIntToByteArray (Int32 dwInput) {
            var rgbTemp = new Byte[8]; // int can never be greater than Int64
            Int32 t1;  // t1 is remaining value to account for
            Int32 t2;  // t2 is t1 % 256
            var i = 0;

            if (dwInput == 0) return new Byte[1]; 
            t1 = dwInput; 
            while (t1 > 0) {
                t2 = t1 % 256;
                rgbTemp[i] = (Byte) t2;
                t1 = (t1 - t2)/256;
                i++;
            }
            // Now, copy only the non-zero part of rgbTemp and reverse
            var rgbOutput = new Byte[i];
            // copy and reverse in one pass
            for (var j = 0; j < i; j++) {
                rgbOutput[j] = rgbTemp[i-j-1];
            }
            return rgbOutput;
        }

        internal static Int32 GetHexArraySize (Byte[] hex) {
            var index = hex.Length;
            while (index-- > 0) {
                if (hex[index] != 0)
                    break;
            }
            return index + 1;
        }

        private static readonly Char[] hexValues = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        internal static String EncodeHexString (Byte[] sArray) {
            return EncodeHexString(sArray, 0, (UInt32) sArray.Length);
        }

        internal static String EncodeHexString (Byte[] sArray, UInt32 start, UInt32 end) {
            String result = null;
            if (sArray != null) {
                var hexOrder = new Char[(end - start) * 2];
                UInt32 digit;
                for (UInt32 i = start, j = 0; i < end; i++) {
                    digit = (UInt32) ((sArray[i] & 0xf0) >> 4);
                    hexOrder[j++] = hexValues[digit];
                    digit = (UInt32) (sArray[i] & 0x0f);
                    hexOrder[j++] = hexValues[digit];
                }
                result = new String(hexOrder);
            }
            return result;
        }

        [SecuritySafeCritical]
        internal static X509Certificate2Collection BuildBagOfCerts (KeyInfoX509Data keyInfoX509Data, CertUsageType certUsageType) {
            var collection = new X509Certificate2Collection();
            var decryptionIssuerSerials = (certUsageType == CertUsageType.Decryption ? new ArrayList() : null);
            if (keyInfoX509Data.Certificates != null) {
                foreach (X509Certificate2 certificate in keyInfoX509Data.Certificates) {
                    switch (certUsageType) {
                    case CertUsageType.Verification:
                        collection.Add(certificate);
                        break;
                    case CertUsageType.Decryption:
                        decryptionIssuerSerials.Add(new X509IssuerSerial(certificate.IssuerName.Name, certificate.SerialNumber));
                        break;
                    }
                }
            }

            if (keyInfoX509Data.SubjectNames == null && keyInfoX509Data.IssuerSerials == null &&
                keyInfoX509Data.SubjectKeyIds == null && decryptionIssuerSerials == null)
                return collection;

            // Open LocalMachine and CurrentUser "Other People"/"My" stores.

            // Assert OpenStore since we are not giving back any certificates to the user.
            var sp = new StorePermission(StorePermissionFlags.OpenStore);
            sp.Assert();

            var stores = new X509Store[2];
            var storeName = (certUsageType == CertUsageType.Verification ? "AddressBook" : "My");
            stores[0] = new X509Store(storeName, StoreLocation.CurrentUser);
            stores[1] = new X509Store(storeName, StoreLocation.LocalMachine);

            for (var index=0; index < stores.Length; index++) {
                if (stores[index] != null) {
                    X509Certificate2Collection filters = null;
                    // We don't care if we can't open the store.
                    try {
                        stores[index].Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                        filters = stores[index].Certificates;
                        stores[index].Close();
                        if (keyInfoX509Data.SubjectNames != null) {
                            foreach (String subjectName in keyInfoX509Data.SubjectNames) {
                                filters = filters.Find(X509FindType.FindBySubjectDistinguishedName, subjectName, false);
                            }
                        }
                        if (keyInfoX509Data.IssuerSerials != null) {
                            foreach (X509IssuerSerial issuerSerial in keyInfoX509Data.IssuerSerials) {
                                filters = filters.Find(X509FindType.FindByIssuerDistinguishedName, issuerSerial.IssuerName, false);
                                filters = filters.Find(X509FindType.FindBySerialNumber, issuerSerial.SerialNumber, false);
                            }
                        }
                        if (keyInfoX509Data.SubjectKeyIds != null) {
                            foreach (Byte[] ski in keyInfoX509Data.SubjectKeyIds) {
                                var hex = EncodeHexString(ski);
                                filters = filters.Find(X509FindType.FindBySubjectKeyIdentifier, hex, false);
                            }
                        }
                        if (decryptionIssuerSerials != null) {
                            foreach (X509IssuerSerial issuerSerial in decryptionIssuerSerials) {
                                filters = filters.Find(X509FindType.FindByIssuerDistinguishedName, issuerSerial.IssuerName, false);
                                filters = filters.Find(X509FindType.FindBySerialNumber, issuerSerial.SerialNumber, false);
                            }
                        }
                    }
                    catch (CryptographicException) {}

                    if (filters != null) 
                        collection.AddRange(filters);
                }
            }

            return collection;
        }
    }
}
