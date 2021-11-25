using System;
using System.Globalization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {iso(1) identified-organization(3) dod(6) internet(1) private(4) enterprise(1) 311 21 4}
     * 1.3.6.1.4.1.311.21.4
     * /ISO/Identified-Organization/6/1/4/1/311/21/4
     * szOID_CRL_NEXT_PUBLISH
     * */
    [Asn1SpecificObject("1.3.6.1.4.1.311.21.4")]
    internal class Asn1NextCRLPublish : Asn1CertificateExtension
        {
        public DateTime Value { get; }
        public Asn1NextCRLPublish(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0)
                    {
                    Value = (Asn1Time)octet[0];
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