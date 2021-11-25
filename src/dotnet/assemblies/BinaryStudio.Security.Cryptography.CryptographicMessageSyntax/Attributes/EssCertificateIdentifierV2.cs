using System;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.PublicKeyInfrastructure;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public class EssCertificateIdentifierV2 : Asn1LinkObject<Asn1Sequence>
        {
        public X509AlgorithmIdentifier HashAlgorithm { get; }
        public ICmsIssuerAndSerialNumber IssuerAndSerial { get; }
        public Byte[] HashValue { get; }
        internal EssCertificateIdentifierV2(Asn1Sequence source)
            : base(source)
            {
            var c = source.Count;
            var i = 0;
            if (source[i] is Asn1Sequence sequence) {
                i++;
                HashAlgorithm = new X509AlgorithmIdentifier(sequence);
                }
            HashValue = source[i].Content.ToArray();
            i++;
            if (i < c) {
                IssuerAndSerial = new CmsIssuerSerialSequence((Asn1Sequence)source[i]);
                }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(HashAlgorithm), HashAlgorithm);
            writer.WriteValue(serializer, nameof(HashValue), HashValue.ToString("X"));
            writer.WriteValue(serializer, nameof(IssuerAndSerial), IssuerAndSerial);
            writer.WriteEndObject();
            }
        }
    }