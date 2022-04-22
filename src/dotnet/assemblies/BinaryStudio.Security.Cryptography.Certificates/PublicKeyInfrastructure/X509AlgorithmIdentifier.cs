using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <pre style="font-family: Consolas">
    /// AlgorithmIdentifier  ::=  SEQUENCE  {
    ///   algorithm               OBJECT IDENTIFIER,
    ///   parameters              ANY DEFINED BY algorithm OPTIONAL
    ///   }
    /// </pre>
    /// </remarks>
    [TypeConverter(typeof(X509AlgorithmIdentifierTypeConverter))]
    [DefaultProperty(nameof(Identifier))]
    public sealed class X509AlgorithmIdentifier: IJsonSerializable,IXmlSerializable
        {
        [TypeConverter(typeof(Asn1ObjectIdentifierTypeConverter))]
        public Asn1ObjectIdentifier Identifier { get; }

        public Object Parameters { get; }

        public X509AlgorithmIdentifier(Asn1Sequence source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var c = source.Count;
            if (c == 0)                               { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (c >  2)                               { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (!(source[0] is Asn1ObjectIdentifier)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            Identifier = (Asn1ObjectIdentifier)source[0];
            if (c == 2) {
                Parameters = X509PublicKeyParameters.From(
                    Identifier.ToString(),
                    source[1]);
                }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Identifier.ToString();
            }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
            //writer.WriteIndentSpace(1);
            writer.WriteComment($"{Identifier.FriendlyName}");
            writer.WriteEndObject();
            }

        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.</summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.</returns>
        XmlSchema IXmlSerializable.GetSchema()
            {
            throw new NotImplementedException();
            }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
            {
            throw new NotImplementedException();
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
            {
            writer.WriteStartElement("X509AlgorithmIdentifier");
            writer.WriteAttributeString(nameof(Identifier), Identifier.ToString());
            writer.WriteEndElement();
            }
        }
    }