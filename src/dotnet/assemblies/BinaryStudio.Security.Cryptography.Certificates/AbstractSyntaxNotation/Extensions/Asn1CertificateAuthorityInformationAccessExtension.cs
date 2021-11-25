using System;
using System.Globalization;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {iso(1) identified-organization(3) dod(6) internet(1) security(5) mechanisms(5) pkix(7) pe(1) authorityInfoAccess(1)}
     * 1.3.6.1.5.5.7.1.1
     * /ISO/Identified-Organization/6/1/5/5/7/1/1
     * Certificate authority information access
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("1.3.6.1.5.5.7.1.1")]
    internal sealed class Asn1CertificateAuthorityInformationAccessExtension : Asn1CertificateExtension
        {
        public Asn1CertificateAuthorityInformationAccessCollection AccessDescriptions { get; }
        public Asn1CertificateAuthorityInformationAccessExtension(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    var sequence = octet[0];
                    AccessDescriptions = new Asn1CertificateAuthorityInformationAccessCollection(
                        sequence.Select(i => new Asn1CertificateAuthorityInformationAccess(i)));
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
            return AccessDescriptions.ToString();
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, nameof(AccessDescriptions), AccessDescriptions);
                }
            }
        }
    }