using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public sealed class Asn1RelativeDistinguishedNameSequence :
        Asn1ReadOnlyCollection<KeyValuePair<Asn1ObjectIdentifier, String>>,
        IJsonSerializable,IX509GeneralName,IXmlSerializable
        {
        public Asn1RelativeDistinguishedNameSequence(IEnumerable<KeyValuePair<Asn1ObjectIdentifier, String>> source)
            : base(source)
            {
            }

        #region M:ToString(Object):String
        internal static String ToString(Object source) {
            if (source == null) { return String.Empty; }
            if (source is String) {
                var value = (String)source;
                if (value.IndexOf("\"") != -1) {
                    return $"\"{value.Replace("\"", "\"\"")}\"";
                    }
                }
            return source.ToString();
            }
        #endregion
        #region M:ToString:String
        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return String.Join(", ", Items.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value)}"));
            }
        #endregion
        #region M:Contains(String,Func<String>):Boolean
        public Boolean Contains(String key, Func<String, Boolean> comparer) {
            if (comparer == null) { throw new ArgumentNullException(nameof(comparer)); }
            foreach (var item in Items) {
                if (item.Key.Equals(key)) {
                    return comparer.Invoke(ToString(item.Value));
                    }
                }
            return false;
            }
        #endregion

        public Object this[String key] { get {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            foreach (var item in Items) {
                if (item.Key.Equals(key)) {
                    return item.Value;
                    }
                }
            return null;
            }}

        public Boolean TryGetValue(String key, out Object r) {
            r = null;
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            foreach (var item in Items) {
                if (item.Key.Equals(key)) {
                    r = item.Value;
                    return true;
                    }
                }
            return false;
            }

        public Object this[Asn1ObjectIdentifier key] { get {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            foreach (var item in Items) {
                if (item.Key.Equals(key)) { return item.Value; }
                }
            throw new ArgumentOutOfRangeException(nameof(key));
            }}

        Boolean IX509GeneralName.IsEmpty { get {
            return Count == 0;
            }}

        public X509GeneralNameType Type { get { return X509GeneralNameType.Directory; }}

        public void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(Count), Count);
                writer.WriteValue(serializer, "(Self)", ToString());
                writer.WritePropertyName("[Self]");
                using (writer.ArrayScope(serializer)) {
                    var values = Items.ToArray();
                    var n = values.Max(i => i.Key.ToString().Length);
                    var formatting = writer.Formatting;
                    writer.Formatting = Formatting.None;
                    for (var i = 0; i < values.Length; i++) {
                        var type = values[i].Key.ToString();
                        if (i == 0)
                            {
                            //writer.WriteIndent();
                            }
                        else
                            {
                            writer.Formatting = Formatting.Indented;
                            }
                        writer.WriteStartObject();
                        writer.Formatting = Formatting.None;
                            writer.WriteValue(serializer, "Type",  type);
                            //writer.WriteIndentSpace(n - type.Length);
                            writer.WriteValue(serializer, "Value", values[i].Value.ToString());
                        writer.WriteEndObject();
                        }
                    writer.Formatting = formatting;
                    }
                }
            }

        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.</summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.</returns>
        XmlSchema IXmlSerializable.GetSchema()
            {
            return null;
            }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
            {
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("RelativeDistinguishedName");
            writer.WriteAttributeString("Value", ToString());
            foreach (var item in Items) {
                writer.WriteStartElement("Attribute");
                writer.WriteAttributeString("Type", item.Key.ToString());
                writer.WriteAttributeString("Value", item.Value);
                writer.WriteEndElement();
                }
            writer.WriteEndElement();
            }
        }
    }