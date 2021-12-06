using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using BinaryStudio.Security.Cryptography;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.Cryptography.Xml;

namespace BinaryStudio.Security.XAdES
    {
    public class XAdESAppendQualifyingUnsignedProperties : XAdESAppendQualifyingProperties
        {
        public ITimeStampProtocolClient TimeStampProtocolClient { get;set; }
        public override void Perform(XmlDocument document, SignedXml source)
            {
            base.Perform(document, source);
            if (TimeStampProtocolClient == null) { throw new InvalidOperationException(); }
            var id = source.Signature.Id;
            var o = source.Signature.ObjectList.OfType<DataObject>().FirstOrDefault(i => i.Id == $"{id}-object");
            var nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("d", XmlDSigSchema);
            nsmgr.AddNamespace("x", XAdESSchema);
            var digestmethod = XAdESSignedXml.XmlDSigToOid(DigestMethod);
            if (o == null) {
                using (var context = new CryptographicContext(digestmethod, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    using (var hashengine = context.CreateHashAlgorithm(digestmethod)) {
                        var xml = source.GetXml();
                        var signatureblob = Convert.FromBase64String(((XmlElement)xml.SelectSingleNode("d:SignatureValue", nsmgr)).InnerText);
                        var digest = hashengine.Compute(signatureblob);
                        Byte[] response;
                        TimeStampProtocolClient.SendRequest(digestmethod, digest, true, out response);
                        id = $"{source.Signature.Id}-properties";
                        var xmldsig = XNamespace.Get(XmlDSigSchema);
                        #region QualifyingProperties
                        var r = new XElement(XName.Get("QualifyingProperties", XAdESSchema),
                            new XAttribute(XNamespace.Xmlns + "dsig", xmldsig),
                            new XAttribute("Target", source.Signature.Id),
                            new XElement(XName.Get("UnsignedProperties", XAdESSchema),
                                new XElement(XName.Get("UnsignedSignatureProperties", XAdESSchema),
                                    new XElement(XName.Get("SignatureTimeStamp", XAdESSchema),
                                        new XElement(XName.Get("HashDataInfo", XAdESSchema)),
                                        new XElement(XName.Get("EncapsulatedTimeStamp", XAdESSchema))
                                ))));
                        #endregion
                        o = new DataObject
                            {
                            Data = document.ReadNode(r.CreateReader()).SelectNodes("."),
                            Id = $"{source.Signature.Id}-object"
                            };
                        source.AddObject(o);
                        }
                    }
                }
            else
                {
                var signatureMethod = source.SignatureMethod;
                var signedproperties = o.Data[0].SelectSingleNode($"//x:SignedProperties[@Id='{id}-properties']", nsmgr);
                var properties = (signedproperties == null)
                    ? (XmlElement)o.Data[0].SelectSingleNode($"//x:QualifyingProperties[@Target='{id}']", nsmgr)
                    : (XmlElement)signedproperties.ParentNode;
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
                        var xml = source.GetXml();
                        var signatureblob = Convert.FromBase64String(((XmlElement)xml.SelectSingleNode("d:SignatureValue", nsmgr)).InnerText);
                        var digest = hashengine.Compute(signatureblob);
                        //TimeStampProtocolClient
                        }
                    }
                }
            }
        }
    }