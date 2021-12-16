using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
#if STD_SIGNED_XML
using System.Security.Cryptography.Xml;
#else
using BinaryStudio.Security.Cryptography.Xml;
#endif
using System.Text;
using System.Xml;
using BinaryStudio.Security.XAdES;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using X509Certificate = BinaryStudio.Security.Cryptography.Certificates.X509Certificate;

namespace Kit
    {
    public class XmlDSigOperations
        {
        protected const String XAdESSchema   = "http://uri.etsi.org/01903/v1.4.1#";
        protected const String XmlDSigSchema = "http://www.w3.org/2000/09/xmldsig#";

        #region M:CreateAttachedMessage(CryptographicContext,IEnumerable<IX509Certificate>,String,String,String)
        public static void CreateAttachedMessage(SCryptographicContext context, IEnumerable<IX509Certificate> certificates, String inputfilename, String outputfilename, String timestamp, XAdESFlags flags) {
            if (certificates == null) { throw new ArgumentNullException(nameof(certificates)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (String.IsNullOrEmpty(inputfilename)) { throw new ArgumentOutOfRangeException(nameof(inputfilename)); }
            var console = true;
            if (!String.IsNullOrEmpty(outputfilename)) {
                console = false;
                if (File.Exists(outputfilename)) {
                    File.Delete(outputfilename);
                    }
                }
            using (var inputfile = File.OpenRead(inputfilename)) {
                using (var outputfile = console
                    ? (Stream)new MemoryStream()
                    : File.OpenWrite(outputfilename))
                    {
                    CreateAttachedMessage(context, certificates, inputfile, outputfile, timestamp, flags);
                    if (console)
                        {
                        Utilities.Hex(((MemoryStream)outputfile).ToArray(), Console.Out);
                        }
                    }
                }
            }
        #endregion
        #region M:CreateAttachedMessage(CryptographicContext,IEnumerable<IX509Certificate>,Stream,Stream,String)
        private static void CreateAttachedMessage(SCryptographicContext context, IEnumerable<IX509Certificate> certificates, Stream inputfile, Stream outputfile, String timestamp, XAdESFlags flags) {
            if (certificates == null) { throw new ArgumentNullException(nameof(certificates)); }
            if (inputfile    == null) { throw new ArgumentNullException(nameof(inputfile));    }
            if (outputfile   == null) { throw new ArgumentNullException(nameof(outputfile));   }
            if (context      == null) { throw new ArgumentNullException(nameof(context));      }
            DateTime? datetime = null;
            if (!String.IsNullOrEmpty(timestamp)) {
                switch (timestamp.ToLower())
                    {
                    case "now":
                        {
                        datetime = DateTime.UtcNow;
                        timestamp = null;
                        }
                        break;
                    case "none":
                        {
                        datetime = null;
                        timestamp = null;
                        }
                        break;
                    default:
                        {
                        datetime = DateTime.UtcNow;
                        }
                        break;
                    }
                }
            var inputdocument = LoadDocument(inputfile);
            var signatures = new List<XmlElement>();
            foreach (var certificate in certificates) {
                var signature = CreateAttachedSignature(context, certificate, inputdocument, datetime, timestamp, flags);
                if (signature != null)
                    {
                    signatures.Add(signature);
                    #if DEBUG
                    var builder = new StringBuilder();
                    using (var writer = XmlWriter.Create(new StringWriter(builder), new XmlWriterSettings
                        {
                        Indent = true,
                        IndentChars = " ",
                        OmitXmlDeclaration = true
                        }))
                        {
                        signature.WriteTo(writer);
                        writer.Flush();
                        Console.WriteLine(builder);
                        }
                    #endif
                    }
                }

            if (signatures.Count > 0) {
                foreach (var signature in signatures) {
                    inputdocument.DocumentElement.AppendChild(signature);
                    }
                var writer = new XmlTextWriter(outputfile, new UTF8Encoding(false))
                    {
                    Formatting = Formatting.Indented,
                    IndentChar = ' ',
                    Indentation = 2
                    };
                inputdocument.WriteTo(writer);
                writer.Flush();
                writer.Close();
                }
            }
        #endregion
        #region M:CreateAttachedSignature(CryptographicContext,IX509Certificate,XmlDocument,DateTime?):XmlElement
        private static XmlElement CreateAttachedSignature(SCryptographicContext context, IX509Certificate certificate, XmlDocument document, DateTime? timestamp, String tspserver, XAdESFlags flags)
            {
            var store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(certificate.Bytes));
            return CreateAttachedSignature(store.Certificates.OfType<X509Certificate2>().First(i => String.Equals(i.Thumbprint,certificate.Thumbprint,StringComparison.OrdinalIgnoreCase)), document, timestamp, tspserver, flags);
            }
        #endregion
        #region M:CreateAttachedSignature(X509Certificate2,XmlDocument,DateTime?):XmlElement
        private static XmlElement CreateAttachedSignature(X509Certificate2 certificate, XmlDocument document, DateTime? timestamp, String tspserver, XAdESFlags flags)
            {
            XAdESSignedXml signedxml;
            if (flags.HasFlag(XAdESFlags.IncludeSigningCertificate))
                {
                var key = new KeyInfo();
                key.AddClause(new KeyInfoX509Data(certificate));
                signedxml = new XAdESSignedXml(document)
                    {
                    KeyInfo = key,
                    SigningKey = certificate.PrivateKey,
                    };
                }
            else
                {
                signedxml = new XAdESSignedXml(document)
                    {
                    SigningKey = certificate.PrivateKey,
                    };
                }
            var digestmethod = CryptographicContext.OIDToXmlDSig(SCryptographicContext.SignatureToHashAlg(certificate.SignatureAlgorithm));
            signedxml.Signature.Id = $"id-{(new Random()).Next(65535):X4}";
            var reference = new Reference(String.Empty)
                {
                DigestMethod = digestmethod
                };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            signedxml.AddReference(reference);
            if (timestamp != null)
                {
                (new XAdESAppendQualifyingSignedProperties
                    {
                    SigningTime = timestamp.Value,
                    DigestMethod = digestmethod
                    }).Perform(document, signedxml);
                }
            signedxml.ComputeSignature();
            var target = signedxml.GetXml();
            if ((tspserver != null) && (flags.HasFlag(XAdESFlags.IncludeTimeStamp))) {
                (new XAdESAppendTimeStamp
                    {
                    DigestMethod = digestmethod,
                    //TimeStampProtocolClient = new TimeStampProtocolClient(tspserver),
                    ExcludeTimeStampSigningCertificate = !(flags.HasFlag(XAdESFlags.IncludeTimeStampCertificate))
                    }).Perform(target);
                }
            return target;
            }
        #endregion
        #region M:LoadDocument(Stream):XmlDocument
        private static XmlDocument LoadDocument(Stream stream)
            {
            var r = new XmlDocument();
            r.Load(stream);
            return r;
            }
        #endregion
        #region M:LoadDocument(Stream,String):XmlDocument
        private static XmlDocument LoadDocument(Stream stream, String inputpath)
            {
            var r = new XmlDocument();
            r.Load(stream);
            SetBaseURI(r, inputpath);
            return r;
            }
        #endregion
        #region M:VerifyMessage(CryptographicContext,String,String,[Out]HashSet<IX509Certificate>)
        public static void VerifyMessage(SCryptographicContext context, String inputfilename, String outputfilename, out HashSet<IX509Certificate> certificates)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (String.IsNullOrEmpty(inputfilename)) { throw new ArgumentOutOfRangeException(nameof(inputfilename)); }
            var console = String.IsNullOrEmpty(outputfilename);
            using (var inputfile = File.OpenRead(inputfilename)) {
                using (var outputfile = console
                    ? (Stream)new MemoryStream()
                    : File.OpenWrite(outputfilename))
                    {
                    VerifyMessage(context, inputfile, outputfile, out certificates, Path.GetDirectoryName(inputfilename));
                    if (console)
                        {
                        Utilities.Hex(((MemoryStream)outputfile).ToArray(), Console.Out);
                        }
                    }
                }
            }
        #endregion
        #region M:VerifyMessage(CryptographicContext,Stream,Stream,[Out]HashSet<IX509Certificate>)
        private static void VerifyMessage(SCryptographicContext context,Stream inputfile, Stream outputfile, out HashSet<IX509Certificate> certificates,String inputpath)
            {
            certificates = new HashSet<IX509Certificate>();
            if (inputfile  == null) { throw new ArgumentNullException(nameof(inputfile));  }
            if (outputfile == null) { throw new ArgumentNullException(nameof(outputfile)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            var inputdocument = LoadDocument(inputfile, inputpath);
            var navigator = inputdocument.CreateNavigator();
            var nodelist = inputdocument.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl);
            if (nodelist.Count == 0) { throw new Exception("Signature not found."); }
            var nsmgr = new XmlNamespaceManager(navigator.NameTable);
            nsmgr.AddNamespace("d", XmlDSigSchema);
            nsmgr.AddNamespace("x", XAdESSchema);
            var signatures = new List<XmlElement>();
            foreach (var signature in nodelist.OfType<XmlElement>().ToArray()) {
                signatures.Add(signature);
                signature.ParentNode.RemoveChild(signature);
                }
            foreach (var signature in signatures) {
                #region Loading Signature
                var signedxml = new XAdESSignedXml(inputdocument);
                signedxml.LoadXml(signature);
                #endregion
                #region Updating References
                var references = new List<Reference>();
                for (var i = 0; i < signedxml.SignedInfo.References.Count; i++) {
                    var reference = (Reference)signedxml.SignedInfo.References[i];
                    if (!String.IsNullOrEmpty(reference.Uri)) {
                        if (reference.Uri.StartsWith("file://")) {
                            var filename = reference.Uri.Substring(7);
                            if (File.Exists(filename)) {
                                var r = new Reference(File.OpenRead(filename)) {
                                    DigestMethod = reference.DigestMethod,
                                    DigestValue = reference.DigestValue,
                                    Type = reference.Type,
                                    Id = reference.Id,
                                    Uri = reference.Uri
                                    };
                                references.Add(r);
                                continue;
                                }
                            }
                        }
                    references.Add(reference);
                    }
                signedxml.SignedInfo.References.Clear();
                foreach (var reference in references) { signedxml.AddReference(reference); }
                #endregion
                if ((signedxml.KeyInfo == null) || (signedxml.KeyInfo.Count == 0)) {
                    IX509Certificate signingcertificate = null;
                    #region SignedProperties:SignedSignatureProperties:SigningCertificate:Cert
                    var n = (XmlElement)signature.SelectSingleNode("//x:SignedProperties/x:SignedSignatureProperties/x:SigningCertificate/x:Cert/x:CertDigest", nsmgr);
                    if (n != null) {
                        signingcertificate = (new CustomCertificateResolver()).Find(Convert.FromBase64String(n.SelectSingleNode("d:DigestValue", nsmgr).InnerText));
                        }
                    if (signingcertificate != null) {
                        if (!signedxml.CheckSignature(new X509Certificate2(signingcertificate.Bytes), false)) {
                            throw new Exception("Signature verification failed.");
                            }
                        }
                    else
                        {
                        throw new Exception("Signature verification failed. Cannot find signing certificate.");
                        }
                    #endregion
                    }
                else
                    {
                    if (!signedxml.CheckSignatureReturningKey(out var signingkey)) { throw new Exception("Signature verification failed."); }
                    if (signedxml.KeyInfo != null) {
                        foreach (var certificate in signedxml.KeyInfo.OfType<KeyInfoX509Data>().SelectMany(i => i.Certificates.OfType<X509Certificate2>())) {
                            certificates.Add(new X509Certificate(certificate.RawData));
                            }
                        }
                    }
                #region UnsignedProperties:UnsignedSignatureProperties:SignatureTimeStamp:EncapsulatedTimeStamp
                var encapsulatedTimeStamp = (XmlElement)signature.SelectSingleNode("//x:EncapsulatedTimeStamp", nsmgr);
                if (encapsulatedTimeStamp != null) {
                    var timestampresponse = Convert.FromBase64String(encapsulatedTimeStamp.InnerText);
                    var digestmethod = XAdESSignedXml.XmlDSigToOid(signedxml.SignatureMethod);
                    using (var localcontext = new SCryptographicContext(digestmethod, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                        localcontext.VerifyAttachedMessageSignature(timestampresponse, out var signers, new CustomCertificateResolver());
                        certificates.UnionWith(signers);
                        }
                    }
                #endregion
                }
            var writer = new XmlTextWriter(outputfile, new UTF8Encoding(false))
                {
                Formatting = Formatting.Indented,
                IndentChar = ' ',
                Indentation = 2
                };
            inputdocument.WriteTo(writer);
            writer.Close();
            }
        #endregion
        #region M:SetBaseURI(XmlDocument,String)
        private static void SetBaseURI(XmlDocument source, String uri) {
            var mi = source.GetType().GetMethod("SetBaseURI", BindingFlags.Instance|BindingFlags.NonPublic);
            if (mi != null) {
                if (String.IsNullOrEmpty(uri)) {
                    uri = Directory.GetCurrentDirectory();
                    }
                mi.Invoke(source, new Object[]{ uri});
                }
            }
        #endregion
        }
    }