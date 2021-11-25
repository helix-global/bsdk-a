using System;
using System.Globalization;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) cRLNumber(20)}
     * 2.5.29.20
     * /Joint-ISO-ITU-T/5/29/20
     * Certificate Revocation List (CRL) number
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.20")]
    internal class Asn1CRLNumberExtension : Asn1CertificateExtension
        {
        public String Value { get; }
        public Asn1CRLNumberExtension(Asn1CertificateExtension source)
            :base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count == 1) {
                    Value = String.Join(String.Empty, octet[0].Content.ToArray().Select(i => i.ToString("X2")));
                    }
                }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, nameof(Value), Value);
                }
            }
        }
    }