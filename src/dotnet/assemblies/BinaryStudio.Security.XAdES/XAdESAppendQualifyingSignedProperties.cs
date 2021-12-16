using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
#if STD_SIGNED_XML
using System.Security.Cryptography.Xml;
#else
using BinaryStudio.Security.Cryptography.Xml;
#endif
using System.Xml.Linq;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;

namespace BinaryStudio.Security.XAdES
    {
    public class XAdESAppendQualifyingSignedProperties : XAdESAppendQualifyingProperties
        {
        public DateTime SigningTime { get;set; }

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

        protected override XElement PerformCore(SignedXml source)
            {
            var id = $"{source.Signature.Id}-properties";
            var r = base.PerformCore(source);
            var certificates =  source.KeyInfo.OfType<KeyInfoX509Data>().SelectMany(i => i.Certificates.OfType<X509Certificate2>()).ToArray();
            if (certificates.Length > 0) {
                var e = new XElement(XName.Get("SignedProperties", XAdESSchema),
                    new XAttribute("Id", id),
                    new XElement(XName.Get("SignedSignatureProperties", XAdESSchema),
                        new XElement(XName.Get("SigningTime",        XAdESSchema), SigningTime.ToString("o")),
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
                        )));
                r.Add(e);
                }
            return r;
            }

        private static X509Certificate2 GetCertificate(AsymmetricAlgorithm source) {
            var pi = source.GetType().GetProperty("ContainerCertificate");
            if (pi != null) {
                #if NET40
                return (X509Certificate2)pi.GetValue(source, null);
                #else
                return (X509Certificate2)pi.GetValue(source);
                #endif
                }
            return null;
            }

        public override void Perform(XmlDocument document, SignedXml source)
            {
            base.Perform(document, source);
            var id = source.Signature.Id;
            var o = source.Signature.ObjectList.OfType<DataObject>().FirstOrDefault(i => i.Id == $"{id}-object");
            if (o == null) {
                var certificates = source.KeyInfo.OfType<KeyInfoX509Data>().SelectMany(i => i.Certificates.OfType<X509Certificate2>()).ToList();
                if (certificates.Count == 0) {
                    if (source.SigningKey == null) { throw new ArgumentOutOfRangeException(nameof(source)); }
                    var certificate = GetCertificate(source.SigningKey);
                    certificates.Add(certificate);
                    }
                if (certificates.Count > 0) {
                    id = $"{source.Signature.Id}-properties";
                    var xmldsig = XNamespace.Get(XmlDSigSchema);
                    #region QualifyingProperties
                    var r = new XElement(XName.Get("QualifyingProperties", XAdESSchema),
                        new XAttribute(XNamespace.Xmlns + "dsig", xmldsig),
                        new XAttribute("Target", source.Signature.Id),
                        new XElement(XName.Get("SignedProperties", XAdESSchema),
                            new XAttribute("Id", id),
                            new XElement(XName.Get("SignedSignatureProperties", XAdESSchema),
                                new XElement(XName.Get("SigningTime",        XAdESSchema), SigningTime.ToString("o")),
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
                    #endregion
                    o = new DataObject
                        {
                        Data = document.ReadNode(r.CreateReader()).SelectNodes("."),
                        Id = $"{source.Signature.Id}-object"
                        };
                    source.AddObject(o);
                    Reference reference;
                    source.AddReference(reference = new Reference($"#{id}")
                        {
                        Type = XAdESSignedProperties,
                        DigestMethod = DigestMethod
                        });
                    reference.AddTransform(new XmlDsigExcC14NTransform());
                    }
                return;
                }
            }
        }
    }