using System;
using System.Globalization;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) extKeyUsage(37)}
     * 2.5.29.37
     * /Joint-ISO-ITU-T/5/29/37
     * Extended key usage 
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.37")]
    internal sealed class Asn1CertificateExtendedKeyUsageExtension : Asn1CertificateExtension
        {
        [Asn1DisplayName("9001")]
        public Asn1ObjectIdentifierCollection Value { get; }
        public Asn1CertificateExtendedKeyUsageExtension(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    Value = new Asn1ObjectIdentifierCollection(octet[0].OfType<Asn1ObjectIdentifier>());
                    }
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString() {
            if (Value != null) {
                return String.Join(";", ((Asn1ObjectIdentifierCollection)Value).Select(Asn1DecodedObjectIdentifierTypeConverter.ToString));
                }
            return "(none)";
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WritePropertyName("[Self]");
                writer.WriteStartArray();
                foreach (var i in Value)
                    {
                    writer.WriteValue(i.ToString());
                    }
                writer.WriteEndArray();
                }
            }
        }
    }