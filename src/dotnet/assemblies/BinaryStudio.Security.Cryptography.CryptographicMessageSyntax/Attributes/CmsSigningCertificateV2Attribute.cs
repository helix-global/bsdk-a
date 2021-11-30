using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /**
     * {iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs-9(9) smime(16) id-aa(2) id-aa-signingCertificateV2(47)}
     * {1.2.840.113549.1.9.16.2.47}
     * {/ISO/Member-Body/US/113549/1/9/16/2/47}
     * RFC5126
     * SigningCertificateV2 ::=  SEQUENCE
     * {
     *   certs        SEQUENCE OF ESSCertIDv2,
     *   policies     SEQUENCE OF PolicyInformation OPTIONAL
     * }
     *
     * ESSCertIDv2 ::=  SEQUENCE
     * {
     *   hashAlgorithm           AlgorithmIdentifier DEFAULT {algorithm id-sha256},
     *   certHash                Hash,
     *   issuerSerial            IssuerSerial OPTIONAL
     * }
     *
     * Hash ::= OCTET STRING
     * IssuerSerial ::= SEQUENCE
     * {
     *   issuer                   GeneralNames,
     *   serialNumber             CertificateSerialNumber
     * }
     */
    [CmsSpecific("1.2.840.113549.1.9.16.2.47")]
    [DefaultProperty(nameof(Certificates))]
    public class CmsSigningCertificateV2Attribute : CmsAttribute
        {
        [Browsable(false)] public override Object Value { get { return base.Value; }}
        public IList<EssCertificateIdentifierV2> Certificates { get; }
        internal CmsSigningCertificateV2Attribute(CmsAttribute o)
            : base(o)
            {
            var r = (Asn1Sequence)Values.FirstOrDefault();
            if (r == null) { throw new ArgumentOutOfRangeException(nameof(o)); }
            var c = r.Count;
            if (c == 0) { throw new ArgumentOutOfRangeException(nameof(o)); }
            Certificates = r[0].Cast<Asn1Sequence>().Select(i => new EssCertificateIdentifierV2(i)).ToArray();
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