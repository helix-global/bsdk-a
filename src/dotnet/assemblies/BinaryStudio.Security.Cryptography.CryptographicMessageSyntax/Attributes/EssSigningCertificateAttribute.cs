using System;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /**
     * {iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs9(9) smime(16) id-aa(2) 12}
     * {1.2.840.113549.1.9.16.2.12}
     *
     * SigningCertificate ::=  SEQUENCE
     * {
     *   certs        SEQUENCE OF ESSCertID,
     *   policies     SEQUENCE OF PolicyInformation OPTIONAL
     * }
     *
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
    [CmsSpecific("1.2.840.113549.1.9.16.2.12")]
    public class EssSigningCertificateAttribute : CmsAttribute
        {
        public IList<EssCertificateIdentifier> Certificates { get; }
        public EssSigningCertificateAttribute(CmsAttribute o)
            : base(o)
            {
            var r = (Asn1Sequence)Values.FirstOrDefault();
            if (r == null) { throw new ArgumentOutOfRangeException(nameof(o)); }
            var c = r.Count;
            if (c == 0) { throw new ArgumentOutOfRangeException(nameof(o)); }
            Certificates = r[0].Cast<Asn1Sequence>().Select(i => new EssCertificateIdentifier(i)).ToArray();
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer) {
            if (!IsNullOrEmpty(Certificates)) {
                writer.WritePropertyName(nameof(Certificates));
                writer.WriteStartArray();
                foreach (var certificate in Certificates) {
                    certificate.WriteJson(writer, serializer);
                    }
                writer.WriteEndArray();
                }
            }
        }
    }