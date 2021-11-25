using System;
using System.Globalization;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    [Asn1SpecificObject("1.2.840.113533.7.65.0")]
    internal class Asn1EntrustVersionInfo : Asn1CertificateExtension
        {
        public String Version { get; }
        public Asn1BitString Flags { get; }
        public Asn1EntrustVersionInfo(Asn1CertificateExtension source)
            :base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    Version = (Asn1String)octet[0][0];
                    Flags = (Asn1BitString)octet[0][1];
                    }
                }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, nameof(Version), Version);
                writer.WriteValue(serializer, nameof(Flags), String.Join(String.Empty, Flags.Content.ToArray().Select(i => i.ToString("X2"))));
                }
            }
        }
    }