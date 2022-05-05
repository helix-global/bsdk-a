using System.Linq;
using System.Security.Cryptography;
using System.Xml;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [Asn1SpecificObject("1.2.840.113549.1.1.10")]
    public sealed class RSAWithPSSSignatureAlgorithm : Asn1SignatureAlgorithm
        {
        public override Oid HashAlgorithm { get; }
        public RSAWithPSSSignatureAlgorithm(Asn1Object source)
            : base(source)
            {
            if (source.Count > 1) {
                var contextspecifics = source[1].OfType<Asn1ContextSpecificObject>().ToArray();
                var contextspecific = contextspecifics.FirstOrDefault(i => i.Type == 0);
                if (contextspecific != null) {
                    HashAlgorithm = new Oid(((Asn1ObjectIdentifier)contextspecific[0][0]).ToString());
                    }
                }
            HashAlgorithm = HashAlgorithm ?? new Oid(szOID_OIWSEC_sha1);
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("RSAWithPSSSignatureAlgorithm");
            writer.WriteAttributeString(nameof(SignatureAlgorithm), SignatureAlgorithm.ToString());
            writer.WriteAttributeString(nameof(HashAlgorithm), HashAlgorithm.Value);
            writer.WriteEndElement();
            }
        }
    }