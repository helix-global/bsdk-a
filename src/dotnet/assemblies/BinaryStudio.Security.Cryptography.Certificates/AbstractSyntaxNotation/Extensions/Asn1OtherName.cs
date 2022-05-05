using System;
using System.Text;
using System.Xml;
using BinaryStudio.Security.Cryptography.Certificates;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal sealed class Asn1OtherName : Asn1GeneralNameObject
        {
        public Asn1ObjectIdentifier Type { get; }
        public Object Value { get; }
        public Asn1OtherName(Asn1ContextSpecificObject source)
            : base(source)
            {
            Type = (Asn1ObjectIdentifier)source[0];
            if (source[1] is Asn1ContextSpecificObject) {
                if (source[1].Count == 1) {
                    if (source[1][0] is Asn1OctetString) {
                        Value = source[1][0].Content.ToArray();
                        Value = Encoding.UTF8.GetString((Byte[])Value);
                        return;
                        }
                    if (source[1][0] is Asn1String value) {
                        Value = value.Value;
                        return;
                        }
                    }
                }
            Value = source[1].Content.ToArray();
            }

        public override Boolean IsEmpty { get {
            return (Value == null) ||
                   (Value is String r) &&
                   String.IsNullOrEmpty(r);
            }}

        public override String ToString()
            {
            return (Value != null)
                ? Value.ToString()
                : "{none}";
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            writer.WriteValue(Value);
            }

        protected override X509GeneralNameType InternalType { get { return X509GeneralNameType.Other; }}

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("GeneralName");
            writer.WriteAttributeString("Type", InternalType.ToString());
            writer.WriteAttributeString("Type.Numeric", ((Int32)InternalType).ToString());
            writer.WriteAttributeString("Value", ToString());
            if (Type != null) {
                writer.WriteAttributeString("ObjectIdentifier", Type.ToString());
                }
            writer.WriteEndElement();
            }
        }
    }