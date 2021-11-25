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
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) certificatePolicies(32)}
     * 2.5.29.32
     * /Joint-ISO-ITU-T/5/29/32
     * Certificate policies
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.32")]
    internal class Asn1CertificatePoliciesExtension : Asn1CertificateExtension
        {
        public Asn1ObjectIdentifierCollection CertificatePolicies { get; }
        public Asn1CertificatePoliciesExtension(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    CertificatePolicies = new Asn1ObjectIdentifierCollection(
                        octet[0].OfType<Asn1Sequence>().
                        Select(i => (Asn1ObjectIdentifier)i[0]));
                    }
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            if (CertificatePolicies  != null) {
                return String.Join(";", CertificatePolicies.Select(Asn1DecodedObjectIdentifierTypeConverter.ToString));
                }
            return base.ToString();
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, "CertificatePolicies", CertificatePolicies);
                }
            }
        }
    }