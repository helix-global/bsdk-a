using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /**
     * ESSCertID ::=  SEQUENCE
     * {
     *   certHash                 Hash,
     *   issuerSerial             IssuerSerial OPTIONAL
     * }
     * Hash ::= OCTET STRING -- SHA1 hash of entire certificate
     * IssuerSerial ::= SEQUENCE
     * {
     *   issuer                   GeneralNames,
     *   serialNumber             CertificateSerialNumber
     * }
     */
    public class EssCertificateIdentifier : Asn1LinkObject<Asn1Sequence>
        {
        internal EssCertificateIdentifier(Asn1Sequence source)
            : base(source)
            {
            return;
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteEndObject();
            }
        }
    }