using System;
using System.Globalization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {iso(1) identified-organization(3) dod(6) internet(1) private(4) enterprise(1) 311 21 1}
     * 1.3.6.1.4.1.311.21.1
     * /ISO/Identified-Organization/6/1/4/1/311/21/1
     * Certificate services Certification Authority (CA) version
     * */
    [Asn1SpecificObject("1.3.6.1.4.1.311.21.1")]
    internal sealed class Asn1CertificateCAVersionExtension : Asn1CertificateExtension
        {
        public Version Version { get; }
        public Asn1CertificateCAVersionExtension(Asn1CertificateExtension source)
            : base(source)
            {
            Version = new Version(0, 0);
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    var value = ((Asn1Integer)octet[0]).Content.ToArray();
                    if (value.Length == 1)
                        {
                        Version = new Version(value[0], 0);
                        }
                    else if (value.Length == 2)
                        {
                        var major = (((UInt16)value[0]) << 8) | (UInt16)value[1];
                        Version = new Version(major, 0);
                        }
                    else if (value.Length == 3)
                        {
                        var major = (((UInt16)value[1]) << 8) | (UInt16)value[2];
                        var minor = value[0];
                        Version = new Version(major, minor);
                        }
                    else if (value.Length == 4)
                        {
                        var major = (((UInt16)value[2]) << 8) | (UInt16)value[3];
                        var minor = (((UInt16)value[0]) << 8) | (UInt16)value[1];
                        Version = new Version(major, minor);
                        }
                    }
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString() {
            return Version.ToString();
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, nameof(Version), Version.ToString());
                }
            }
        }
    }