using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    [Asn1SpecificObject("2.5.29.14")]
    public sealed class CertificateSubjectKeyIdentifier : Asn1CertificateExtension
        {
        public Byte[] KeyIdentifier { get; }
        public CertificateSubjectKeyIdentifier(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    octet = (Asn1OctetString)octet[0];
                    KeyIdentifier = octet.Content.ToArray();
                    }
                else
                    {
                    KeyIdentifier = octet.Content.ToArray();
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
            return KeyIdentifier.ToString("x");
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("Extension");
            writer.WriteAttributeString(nameof(Identifier), Identifier.ToString());
            writer.WriteAttributeString(nameof(IsCritical), IsCritical.ToString());
            writer.WriteStartElement("Extension.Value");
            writer.WriteStartElement("CertificateSubjectKeyIdentifier");
            if (KeyIdentifier != null) {
                writer.WriteAttributeString("Key", KeyIdentifier.ToString("x"));
                }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, "KeyIdentifier", KeyIdentifier.ToString("x"));
                }
            }
        }
    }