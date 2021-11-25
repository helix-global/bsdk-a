using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates.Converters;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [TypeConverter(typeof(X509RelativeDistinguishedNameSequenceTypeConverter))]
    #if USE_WINFORMS
    [Editor(typeof(NoEditor), typeof(UITypeEditor))]
    #endif
    public class X509RelativeDistinguishedNameSequence : Asn1ReadOnlyCollection<KeyValuePair<Asn1ObjectIdentifier, Object>>, IXmlSerializable
        {
        private class X509RelativeDistinguishedNamePropertyDescriptor : PropertyDescriptor
            {
            private Object Value { get; }
            public X509RelativeDistinguishedNamePropertyDescriptor(Object value, String name, Attribute[] attributes)
                : base(name, attributes)
                {
                Value = value;
                }

            #region M:CanResetValue(Object):Boolean
            /**
             * <summary>When overridden in a derived class, returns whether resetting an object changes its value.</summary>
             * <param name="component">The component to test for reset capability.</param>
             * <returns>true if resetting the component changes its value; otherwise, false.</returns>
             * */
            public override Boolean CanResetValue(Object component)
                {
                return false;
                }
            #endregion
            #region M:GetValue(Object):Object
            /**
             * <summary>When overridden in a derived class, gets the current value of the property on a component.</summary>
             * <param name="component">The component with the property for which to retrieve the value.</param>
             * <returns>The value of a property for a given component.</returns>
             * */
            public override Object GetValue(Object component)
                {
                return Value;
                }
            #endregion
            #region M:ResetValue(Object)
            /**
             * <summary>When overridden in a derived class, resets the value for this property of the component to the default value.</summary>
             * <param name="component">The component with the property value that is to be reset to the default value.</param>
             * */
            public override void ResetValue(Object component)
                {
                throw new NotImplementedException();
                }
            #endregion
            #region M:SetValue(Object,Object)
            /**
             * <summary>When overridden in a derived class, sets the value of the component to a different value.</summary>
             * <param name="component">The component with the property value that is to be set.</param>
             * <param name="value">The new value.</param>
             * */
            public override void SetValue(Object component, Object value)
                {
                throw new NotImplementedException();
                }
            #endregion
            #region M:ShouldSerializeValue(Object):Boolean
            /**
             * <summary>When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.</summary>
             * <param name="component">The component with the property to be examined for persistence.</param>
             * <returns>true if the property should be persisted; otherwise, false.</returns>
             * */
            public override Boolean ShouldSerializeValue(Object component)
                {
                return false;
                }
            #endregion

            public override Type ComponentType { get { return typeof(X509RelativeDistinguishedNameSequence); }}
            public override Boolean IsReadOnly { get { return true; }}
            public override Type PropertyType  { get { return (Value != null) ? Value.GetType() : typeof(Object); }}
            public override String DisplayName { get {
                return X509DecodedObjectIdentifierTypeConverter.ToString(new Oid(Name));
                }}

            /**
             * <summary>Returns a string that represents the current object.</summary>
             * <returns>A string that represents the current object.</returns>
             * <filterpriority>2</filterpriority>
             * */
            public override String ToString()
                {
                return Name;
                }
            }

        public X509RelativeDistinguishedNameSequence(IEnumerable<KeyValuePair<Asn1ObjectIdentifier, Object>> source)
            : base(source)
            {
            }

        public X509RelativeDistinguishedNameSequence(IEnumerable<KeyValuePair<Asn1ObjectIdentifier, String>> source)
            : base(source.Select(i => new KeyValuePair<Asn1ObjectIdentifier, Object>(i.Key, i.Value)))
            {
            }

        public X509RelativeDistinguishedNameSequence()
            : base(new Dictionary<Asn1ObjectIdentifier, Object>())
            {
            }

        protected override PropertyDescriptorCollection EnsureOverride()
            {
            var r = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            foreach (var pair in Items) {
                r.Add(new X509RelativeDistinguishedNamePropertyDescriptor(
                    pair.Value,
                    pair.Key.ToString(),
                    new Attribute[0]));
                }
            return r;
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
                if (item.Key.Equals(key)) { return item.Value; }
                }
            throw new ArgumentOutOfRangeException(nameof(key));
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

        /**
         * <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.</summary>
         * <returns>An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.</returns>
         */
        XmlSchema IXmlSerializable.GetSchema()
            {
            throw new NotImplementedException();
            }

        /**
         * <summary>Generates an object from its XML representation.</summary>
         * <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
         */
        void IXmlSerializable.ReadXml(XmlReader reader)
            {
            throw new NotImplementedException();
            }

        private static void WriteRaw(XmlWriter writer, String value) {
            var fi = writer.GetType().GetField("writer", BindingFlags.Instance|BindingFlags.NonPublic);
            if (fi != null) {
                var o = fi.GetValue(writer);
                if (o != null) {
                    fi = o.GetType().GetField("writer", BindingFlags.Instance|BindingFlags.NonPublic);
                    if (fi != null) {
                        WriteRaw(fi.GetValue(o) as TextWriter, value);
                        }
                    }
                }
            }

        private static void WriteRaw(TextWriter writer, String value) {
            if (writer != null) {
                writer.Write(value);
                writer.Flush();
                }
            }

        /**
         * <summary>Converts an object into its XML representation.</summary>
         * <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
         */
        void IXmlSerializable.WriteXml(XmlWriter writer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartElement("RelativeDistinguishedNameSequence");
            writer.WriteAttributeString(nameof(Count), Count.ToString());
            var n = Items.Max(i => i.Key.ToString().Length);
            foreach (var item in Items) {
                writer.WriteStartElement("RelativeDistinguishedName");
                var type = item.Key.ToString();
                writer.WriteAttributeString("Type",  type);
                writer.Flush();
                WriteRaw(writer, new String(' ', n - type.Length));
                writer.WriteAttributeString("Value", item.Value.ToString());
                writer.WriteEndElement();
                }
            WriteRaw(writer, "<![CDATA[");
            writer.WriteCData("!!!!");
            writer.WriteEndElement();
            }
        }
    }