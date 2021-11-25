using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;

namespace BinaryStudio.Security.Cryptography.Xml
    {
    public static class SignedXmlTools
        {
        private static String GetDigestMethodFromOid(Oid source) {
            if (source != null) {
                switch (source.Value) {
                    case ObjectIdentifiers.szOID_CP_GOST_R3410_12_512: return CryptographicContext.URN_GOST_DIGEST_2012_512;
                    case ObjectIdentifiers.szOID_CP_GOST_R3411_12_512: return CryptographicContext.URN_GOST_DIGEST_2012_512;
                    }
                }
            return null;
            }

        public static void CreateAttachedSignature(IList<IX509Certificate> certificates, XmlDocument document)
            {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }
            if (certificates == null) { throw new ArgumentNullException(nameof(certificates)); }
            if (certificates.Count == 0) { throw new ArgumentOutOfRangeException(nameof(certificates)); }
            if (document.DocumentElement == null) { throw new ArgumentOutOfRangeException(nameof(document), "Документ не может быть пустым."); }
            try
                {
                var blocks = new List<XmlElement>();
                foreach (var certificate in certificates)
                    {
                    var signedXml = new SignedXml(document);
                    var reference = new Reference { Uri = "" };
                    reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                    reference.AddTransform(new XmlDsigC14NTransform());
                    reference.DigestMethod = GetDigestMethodFromOid(certificate.HashAlgorithm);
                    signedXml.AddReference(reference);
                    signedXml.SigningCertificate = certificate;
                    var keyInfo = new KeyInfo();
                    var certificateData = new KeyInfoX509Data(certificate.Bytes);
                    keyInfo.AddClause(certificateData);
                    signedXml.KeyInfo = keyInfo;
                    signedXml.Signature.SignedInfo.SignatureMethod = CryptographicContext.OIDToXmlDSig(certificate.SignatureAlgorithm);
                    signedXml.ComputeSignature();
                    blocks.Add(signedXml.GetXml());
                    }

                foreach (var block in blocks) {
                    document.LastChild.AppendChild(block);
                    }
                }
            catch
                {
                throw;
                }
            }

        public static void VerifySignature(XmlDocument document, out IList<IX509Certificate> certificates, IX509CertificateResolver resolver)
            {
            if (document == null) { throw new ArgumentNullException(nameof(document)); }
            var nodeList = document.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl);
            if (nodeList.Count == 0) { throw new ArgumentOutOfRangeException(nameof(document), "Электронная подпись не найдена."); }
            certificates = new IX509Certificate[0];
            
            for (var i = 0; i < nodeList.Count; i++) {
                var signature = (XmlElement)nodeList[i];
                var signedXml = new SignedXml(document);
                signedXml.LoadXml(signature);
                var references = new List<Reference>();
                for (var j = 0; j < signedXml.SignedInfo.References.Count; j++) {
                    var reference = (Reference)signedXml.SignedInfo.References[j];
                    if (!String.IsNullOrEmpty(reference.Uri)) {
                        if (reference.Uri.StartsWith("file://")) {
                            var filename = reference.Uri.Substring(7);
                            if (File.Exists(filename)) {
                                var r = new Reference(File.OpenRead(filename))
                                    {
                                    DigestMethod = reference.DigestMethod,
                                    DigestValue = reference.DigestValue,
                                    Type = reference.Type,
                                    Id = reference.Id,
                                    Uri = reference.Uri,
                                    TransformChain = reference.TransformChain
                                    };
                                references.Add(r);
                                continue;
                                }
                            }
                        }
                    references.Add(reference);
                    }
                signedXml.SignedInfo.References.Clear();
                foreach (var reference in references)
                    {
                    signedXml.AddReference(reference);
                    Debug.Print($"digestvalue:{Convert.ToBase64String(reference.DigestValue)}");
                    }

                if (!signedXml.CheckSignature())
                    {
                    throw new ArgumentOutOfRangeException(nameof(document), "Signature validation error.");
                    }
                }
            }
        }
    }