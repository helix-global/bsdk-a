using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    [Asn1SpecificObject("2.5.29.27")]
    internal class Asn1DeltaCRLIndicator : Asn1CertificateExtension
        {
        public BigInteger MinimumBaseCRLNumber { get; }
        public Asn1DeltaCRLIndicator(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    MinimumBaseCRLNumber = (Asn1Integer)octet[0];
                    }
                }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, nameof(MinimumBaseCRLNumber), String.Join(
                    String.Empty,
                    MinimumBaseCRLNumber.
                        ToByteArray().
                        Reverse().
                        Select(i => i.ToString("X2"))));
                }
            }
        }
    }