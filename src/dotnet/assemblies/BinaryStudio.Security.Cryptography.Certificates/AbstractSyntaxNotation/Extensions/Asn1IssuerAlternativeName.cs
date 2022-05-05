using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) issuerAltName(18)}
     * 2.5.29.18
     * /Joint-ISO-ITU-T/5/29/18
     * Issuer alternative name
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.18")]
    public class Asn1IssuerAlternativeName : Asn1CertificateExtension
        {
        public IList<IX509GeneralName> AlternativeName { get; }
        public Asn1IssuerAlternativeName(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0)
                    {
                    AlternativeName = new ReadOnlyCollection<IX509GeneralName>(octet[0].
                        OfType<Asn1ContextSpecificObject>().
                        Select(X509GeneralName.From).
                        Where(i => !i.IsEmpty).
                        ToArray());
                    }
                }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return ((AlternativeName != null) && (AlternativeName.Count > 0))
                ? String.Join(";", AlternativeName.Select(i => $"{{{X509GeneralName.ToString(i.Type)}}}:{{{i}}}"))
                : "{none}";
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WritePropertyName(nameof(AlternativeName));
                if (!IsNullOrEmpty(AlternativeName)) {
                    using (writer.ArrayScope(serializer)) {
                        foreach (var name in AlternativeName.OfType<IJsonSerializable>()) {
                            name.WriteJson(writer, serializer);
                            }
                        }
                    }
                else
                    {
                    writer.WriteValue("(No alternative name)");
                    }
                }
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
            {
            writer.WriteStartElement("Extension");
            writer.WriteAttributeString(nameof(Identifier), Identifier.ToString());
            writer.WriteAttributeString(nameof(IsCritical), IsCritical.ToString());
            if (!IsNullOrEmpty(AlternativeName)) {
                writer.WriteStartElement("Extension.Value");
                writer.WriteStartElement(nameof(AlternativeName));
                foreach (var name in AlternativeName.OfType<IXmlSerializable>()) {
                    name.WriteXml(writer);
                    }
                writer.WriteEndElement();
                writer.WriteEndElement();
                }
            writer.WriteEndElement();
            }
        }
    }