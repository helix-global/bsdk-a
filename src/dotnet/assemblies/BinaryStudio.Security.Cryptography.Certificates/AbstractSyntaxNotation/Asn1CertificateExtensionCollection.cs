using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Xml.Serialization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [XmlRoot("Extensions")]
    public class Asn1CertificateExtensionCollection : Asn1ReadOnlyCollection<Asn1CertificateExtension>, IJsonSerializable
        {
        private class X509CertificateExtensionPropertyDescriptor : PropertyDescriptor
            {
            private Object Value { get; }
            public X509CertificateExtensionPropertyDescriptor(Asn1CertificateExtension source, Attribute[] attributes)
                : base(source.Identifier.ToString(), attributes)
                {
                Value = source;
                }

            #region M:CanResetValue(Object):Boolean
            public override Boolean CanResetValue(Object component)
                {
                return false;
                }
            #endregion
            #region M:GetValue(Object):Object
            public override Object GetValue(Object component)
                {
                return Value;
                }
            #endregion
            #region M:ResetValue(Object)
            public override void ResetValue(Object component)
                {
                throw new NotImplementedException();
                }
            #endregion
            #region M:SetValue(Object,Object)
            public override void SetValue(Object component, Object value)
                {
                throw new NotImplementedException();
                }
            #endregion
            #region M:ShouldSerializeValue(Object):Boolean
            public override Boolean ShouldSerializeValue(Object component)
                {
                return false;
                }
            #endregion

            public override Type ComponentType { get { return typeof(Asn1CertificateExtensionCollection); }}
            public override Boolean IsReadOnly { get { return true; }}
            public override Type PropertyType  { get { return (Value != null) ? Value.GetType() : typeof(Object); }}
            public override String DisplayName { get {
                return Asn1DecodedObjectIdentifierTypeConverter.ToString(new Oid(Name));;
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

        public Asn1CertificateExtensionCollection(IEnumerable<Asn1CertificateExtension> source)
            : base(source)
            {
            }

        internal Asn1CertificateExtensionCollection()
            : base(new List<Asn1CertificateExtension>())
            {
            }

        protected override PropertyDescriptorCollection EnsureOverride() {
            var r = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            foreach (var i in Items) {
                r.Add(new X509CertificateExtensionPropertyDescriptor(
                    i,
                    new Attribute[0]));
                }
            return r;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"{Resources.Count} = {Count}";
            }

        #region P:this[UInt32]:X509CertificateExtension
        public Asn1CertificateExtension this[UInt32 i] { get {
            return base[(Int32)i];
            }}
        #endregion
        #region P:this[String]:X509CertificateExtension
        public Asn1CertificateExtension this[String key] { get {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (String.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key)); }
            foreach (var item in Items) {
                if (item.Identifier.Equals(key)) { return item; }
                }
            throw new ArgumentOutOfRangeException(nameof(key));
            }}
        #endregion
        #region M:HasKey(String):Boolean
        public Boolean HasKey(String key)
            {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            if (String.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key)); }
            foreach (var item in Items) {
                if (item.Identifier.Equals(key)) { return true; }
                }
            return false;
            }
        #endregion

        public void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(Count), Count);
                writer.WritePropertyName("[Self]");
                using (writer.ArrayScope(serializer)) {
                    foreach (var item in Items) {
                        item.WriteJson(writer, serializer);
                        }
                    }
                }
            }
        }
    }