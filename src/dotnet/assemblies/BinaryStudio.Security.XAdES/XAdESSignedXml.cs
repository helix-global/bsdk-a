using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
#if STD_SIGNED_XML
using System.Security.Cryptography.Xml;
#else
using BinaryStudio.Security.Cryptography.Xml;
#endif
using System.Xml;
using System.Xml.Linq;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.XAdES.Internal;

namespace BinaryStudio.Security.XAdES
    {
    public class XAdESSignedXml : SignedXml
        {
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

        public XAdESSignedXml(XmlDocument document)
            :base(document)
            {
            }

        public static Oid XmlDSigToOid(String value)
            {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            switch (value) {
                case XmlDsigSHA256: { return new Oid(ObjectIdentifiers.szOID_NIST_sha256); }
                case XmlDsigSHA384: { return new Oid(ObjectIdentifiers.szOID_NIST_sha384); }
                case XmlDsigSHA512: { return new Oid(ObjectIdentifiers.szOID_NIST_sha512); }
                case URN_GOST_DIGEST_2012_256: { return new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411_12_256);       }
                case URN_GOST_DIGEST_2012_512: { return new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411_12_512);       }
                case URN_GOST_SIGN_2012_256:   { return new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411_12_256_R3410); }
                case URN_GOST_SIGN_2012_512:   { return new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411_12_512_R3410); }
                default: throw new ArgumentOutOfRangeException(nameof(value));
                }
            }

        private static IEnumerable<XmlElement> GetNodes(IEnumerable source) {
            foreach (var i in source.OfType<XmlElement>()) {
                yield return i;
                if (i.HasChildNodes) {
                    foreach (var j in GetNodes(i.ChildNodes)) {
                        yield return j;
                        }
                    }
                }
            }

        public override XmlElement GetIdElement(XmlDocument document, String id) {
            var r = base.GetIdElement(document, id);
            if (r == null) {
                if (Signature != null) {
                    foreach (DataObject o in Signature.ObjectList) {
                        foreach (var i in GetNodes(o.Data)) {
                            var j = i.GetAttributeNode("Id");
                            if (j != null) {
                                if (j.Value == id) {
                                    return i;
                                    }
                                }
                            }
                        }
                    }
                }
            return r;
            }

        /*
        private const String XmlDSigSchema = "http://www.w3.org/2000/09/xmldsig#";
        private const String XAdESSchema   = "http://uri.etsi.org/01903/v1.4.1#";
        private const String XAdESSignedProperties = "http://uri.etsi.org/01903#SignedProperties";

        private XAdESXmlDocument document;

        public XAdESSignedXml(XmlDocument document)
            :base(document)
            {
            this.document = new XAdESXmlDocument(document);
            Resolver = new XAdESXmlResolver();
            }

        #region M:FromHex(Char):Byte
        internal static Byte FromHex(Char source)
            {
            return ((source >= '0') && (source <= '9'))
                ? (Byte)(source - '0')
                : ((source >= 'a') && (source <= 'f'))
                    ? (Byte)(source - 'a' + 10)
                    : ((source >= 'A') && (source <= 'F'))
                        ? (Byte)(source - 'A' + 10)
                        : (Byte)0;
            }
        #endregion
        #region M:ToByteArray(String):Byte[]
        private static Byte[] ToByteArray(String source)
            {
            var r = new List<Byte>();
            source = source.Replace(" ", String.Empty);
            for (var i = 0; i < source.Length; i += 2)
                {
                r.Add((Byte)(
                    (FromHex(source[i]) << 4) |
                    (FromHex(source[i + 1]))));
                }
            return r.ToArray();
            }
        #endregion

        public void AppendQualifyingProperties(DateTime signingtime)
            {
            Signature.Id = Signature.Id ?? $"xmldsig-signature";
            var certificates =  KeyInfo.OfType<KeyInfoX509Data>().SelectMany(i => i.Certificates.OfType<X509Certificate2>()).ToArray();
            var id = $"{Signature.Id}-properties";
            var xmldsig = XNamespace.Get(XmlDSigSchema);
            var r =
            new XElement(XName.Get("QualifyingProperties", XAdESSchema),
                new XAttribute(XNamespace.Xmlns + "xmldsig", xmldsig),
                new XAttribute("Target", Signature.Id),
                new XElement(XName.Get("SignedProperties", XAdESSchema),
                    new XAttribute("Id", id),
                    new XElement(XName.Get("SignedSignatureProperties", XAdESSchema),
                        new XElement(XName.Get("SigningTime",        XAdESSchema), signingtime.ToString("o")),
                        new XElement(XName.Get("SigningCertificate", XAdESSchema),
                            certificates.Select(i =>
                                {
                                return new XElement(XName.Get("Cert", XAdESSchema),
                                    new XElement(XName.Get("CertDigest", XAdESSchema),
                                        new XElement(XName.Get("DigestMethod", XmlDSigSchema), new XAttribute("Algorithm", "http://www.w3.org/2001/04/xmlenc#sha1")),
                                        new XElement(XName.Get("DigestValue",  XmlDSigSchema), Convert.ToBase64String(ToByteArray(i.Thumbprint)))),
                                    new XElement(XName.Get("IssuerSerial", XAdESSchema),
                                        new XElement(XName.Get("X509IssuerName",   XmlDSigSchema), i.IssuerName.Name),
                                        new XElement(XName.Get("X509SerialNumber", XmlDSigSchema), i.SerialNumber))
                                        );
                                }).OfType<Object>().ToArray()
                        ))));
            var o = new DataObject
                {
                Data = document.Source.ReadNode(r.CreateReader()).SelectNodes(".")
                };
            AddObject(o);
            Reference reference;
            AddReference(reference = new Reference($"#{id}")
                {
                Type = XAdESSignedProperties,
                DigestMethod = CryptographicContext.OIDToXmlDSig(CryptographicContext.SignatureToHashAlg(certificates[0].SignatureAlgorithm))
                });
            reference.AddTransform(new XmlDsigExcC14NTransform());
            }
        
        private static IEnumerable<XmlElement> GetNodes(IEnumerable source) {
            foreach (var i in source.OfType<XmlElement>()) {
                yield return i;
                if (i.HasChildNodes) {
                    foreach (var j in GetNodes(i.ChildNodes)) {
                        yield return j;
                        }
                    }
                }
            }

        public override XmlElement GetIdElement(XmlDocument document, String id) {
            var r = base.GetIdElement(document, id);
            if (r == null) {
                if (Signature != null) {
                    foreach (DataObject o in Signature.ObjectList) {
                        foreach (var i in GetNodes(o.Data)) {
                            var j = i.GetAttributeNode("Id");
                            if (j != null) {
                                if (j.Value == id) {
                                    return i;
                                    }
                                }
                            }
                        }
                    }
                }
            return r;
            }*/
        }
    }
