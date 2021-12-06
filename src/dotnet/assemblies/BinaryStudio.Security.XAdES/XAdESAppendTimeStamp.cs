using System;
using System.Xml;
using BinaryStudio.Security.Cryptography;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;

namespace BinaryStudio.Security.XAdES
    {
    public class XAdESAppendTimeStamp : XAdESOperation
        {
        public ITimeStampProtocolClient TimeStampProtocolClient { get;set; }
        public Boolean ExcludeTimeStampSigningCertificate { get;set; }
        public String DigestMethod { get;set; }

        public void Perform(XmlElement target) {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            if (TimeStampProtocolClient == null) { throw new InvalidOperationException(); }
            var nsmgr = new XmlNamespaceManager(target.OwnerDocument.NameTable);
            nsmgr.AddNamespace("d", XmlDSigSchema);
            nsmgr.AddNamespace("x", XAdESSchema);
            var signature = ((target.Name == "Signature") && (target.NamespaceURI == XmlDSigSchema))
                ? target
                : (XmlElement)target.OwnerDocument.SelectSingleNode("//d:Signature", nsmgr);
            if (signature != null)
                {
                var digestmethod = XAdESSignedXml.XmlDSigToOid(DigestMethod);
                var document = signature.OwnerDocument;
                var id = signature.GetAttribute("Id");
                var signedproperties = target.SelectSingleNode($"//x:SignedProperties[@Id='{id}-properties']", nsmgr);
                var properties = (signedproperties == null)
                    ? (XmlElement)target.SelectSingleNode($"//x:QualifyingProperties[@Target='{id}']", nsmgr)
                    : (XmlElement)signedproperties.ParentNode;
                if (properties == null)
                    {
                    var o = (XmlElement)target.SelectSingleNode($"//d:Object[@Id='{id}-object']", nsmgr);
                    if (o == null)
                        {
                        o = (XmlElement)signature.AppendChild(document.CreateElement("Object", XmlDSigSchema));
                        o.SetAttribute("Id", $"{id}-object");
                        properties = (XmlElement)o.AppendChild(document.CreateElement("QualifyingProperties", XAdESSchema));
                        properties.SetAttribute("Target", id);
                        }
                    }
                var unsignedproperties = (XmlElement)properties.SelectSingleNode("/x:UnsignedProperties", nsmgr);
                unsignedproperties?.ParentNode?.RemoveChild(unsignedproperties);
                unsignedproperties = (XmlElement)properties.AppendChild(document.CreateElement("UnsignedProperties", XAdESSchema));
                var unsignedSignatureProperties = unsignedproperties.AppendChild(document.CreateElement("UnsignedSignatureProperties", XAdESSchema));
                var signatureTimeStamp = unsignedSignatureProperties.AppendChild(document.CreateElement("SignatureTimeStamp", XAdESSchema));
                var hashDataInfo = (XmlElement)signatureTimeStamp.AppendChild(document.CreateElement("HashDataInfo", XAdESSchema));
                var encapsulatedTimeStamp = (XmlElement)signatureTimeStamp.AppendChild(document.CreateElement("EncapsulatedTimeStamp", XAdESSchema));
                hashDataInfo.SetAttribute("uri", $"#{id}-value");
                using (var context = new CryptographicContext(digestmethod, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    using (var hashengine = context.CreateHashAlgorithm(digestmethod)) {
                        var signaturevalue = (XmlElement)target.SelectSingleNode("d:SignatureValue", nsmgr);
                        signaturevalue.SetAttribute("Id", $"#{id}-value");
                        var signatureblob = Convert.FromBase64String(signaturevalue.InnerText);
                        var digest = hashengine.Compute(signatureblob);
                        TimeStampProtocolClient.SendRequest(digestmethod, digest, !ExcludeTimeStampSigningCertificate, out Byte[] response);
                        encapsulatedTimeStamp.InnerText = $"\n{Convert.ToBase64String(response, Base64FormattingOptions.InsertLineBreaks)}\n";
                        }
                    }
                }
            }
        }
    }