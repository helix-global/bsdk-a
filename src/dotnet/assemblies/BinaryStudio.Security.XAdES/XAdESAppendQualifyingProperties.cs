using System;
using System.Security.Cryptography;
#if STD_SIGNED_XML
using System.Security.Cryptography.Xml;
#else
using BinaryStudio.Security.Cryptography.Xml;
#endif
using System.Xml;
using System.Xml.Linq;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;

namespace BinaryStudio.Security.XAdES
    {
    public abstract class XAdESAppendQualifyingProperties
        {
        protected const String XmlDSigSchema         = "http://www.w3.org/2000/09/xmldsig#";
        protected const String XAdESSchema           = "http://uri.etsi.org/01903/v1.4.1#";
        protected const String XAdESSignedProperties = "http://uri.etsi.org/01903#SignedProperties";
        public const String XmlDsigSHA1                      = "http://www.w3.org/2000/09/xmldsig#sha1";
        public const String XmlDsigDSA                       = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        public const String XmlDsigRSASHA1                   = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        public const String XmlDsigHMACSHA1                  = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
        public const String XmlDsigSHA256                    = "http://www.w3.org/2001/04/xmlenc#sha256";
        public const String XmlDsigRSASHA256                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        public const String XmlDsigSHA384                    = "http://www.w3.org/2001/04/xmldsig-more#sha384";
        public const String XmlDsigRSASHA384                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
        public const String XmlDsigSHA512                    = "http://www.w3.org/2001/04/xmlenc#sha512";
        public const String XmlDsigRSASHA512                 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
        public const String URI_GOST_CIPHER	                 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gost28147";
        public const String	URI_GOST_DIGEST	                 = "http://www.w3.org/2001/04/xmldsig-more#gostr3411";
        public const String	URI_GOST_HMAC_GOSTR3411	         = "http://www.w3.org/2001/04/xmldsig-more#hmac-gostr3411";
        public const String	URI_GOST_SIGN                    = "http://www.w3.org/2001/04/xmldsig-more#gostr34102001-gostr3411";
        public const String	URI_GOST_TRANSPORT               = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2001";
        public const String	URI_GOST_TRANSPORT_GOST_2012_256 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2012-256";
        public const String	URI_GOST_TRANSPORT_GOST_2012_512 = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2012-512";
        public const String	URN_GOST_DIGEST                  = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr3411";
        public const String	URN_GOST_DIGEST_2012_256         = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-256";
        public const String	URN_GOST_DIGEST_2012_512         = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-512";
        public const String	URN_GOST_HMAC_GOSTR3411          = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:hmac-gostr3411";
        public const String	URN_GOST_SIGN                    = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102001-gostr3411";
        public const String	URN_GOST_SIGN_2012_256           = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-256";
        public const String	URN_GOST_SIGN_2012_512           = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34102012-gostr34112012-512";

        public String DigestMethod { get;set; }

        #region M:OIDToXmlDSig(Oid):String
        public static String OIDToXmlDSig(Oid algid)
            {
            if (algid == null) { throw new ArgumentNullException(nameof(algid)); }
            switch (algid.Value) {
                case ObjectIdentifiers.szOID_NIST_sha256: { return XmlDsigSHA256; }
                case ObjectIdentifiers.szOID_NIST_sha384: { return XmlDsigSHA384; }
                case ObjectIdentifiers.szOID_NIST_sha512: { return XmlDsigSHA512; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_256: { return URN_GOST_DIGEST_2012_256; }
                case ObjectIdentifiers.szOID_CP_GOST_R3411_12_512: { return URN_GOST_DIGEST_2012_512; }
                default: throw new ArgumentOutOfRangeException(nameof(algid));
                }
            }
        #endregion

        protected virtual XElement PerformCore(SignedXml source)
            {
            if (String.IsNullOrWhiteSpace(source.Signature.Id)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            var xmldsig = XNamespace.Get(XmlDSigSchema);
            var r =
            new XElement(XName.Get("QualifyingProperties", XAdESSchema),
                new XAttribute(XNamespace.Xmlns + "dsig", xmldsig),
                new XAttribute("Target", source.Signature.Id));
            return r;
            }

        public virtual void Perform(XmlDocument document, SignedXml source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (String.IsNullOrWhiteSpace(source.Signature.Id)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            //var id = $"{source.Signature.Id}-properties";
            //var r = PerformCore(source);
            //if (r != null) {
            //    var o = new DataObject
            //        {
            //        Data = document.ReadNode(r.CreateReader()).SelectNodes("."),
            //        Id = $"{source.Signature.Id}-object"
            //        };
            //    source.AddObject(o);
            //    Reference reference;
            //    source.AddReference(reference = new Reference($"#{id}")
            //        {
            //        Type = XAdESSignedProperties,
            //        DigestMethod = DigestMethod
            //        });
            //    reference.AddTransform(new XmlDsigExcC14NTransform());
            //    }
            }
        }
    }