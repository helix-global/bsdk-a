using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

//namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
//    {
//    [TypeConverter(typeof(X509RelativeDistinguishedNameSequenceTypeConverter))]
//    #if USE_WINFORMS
//    [Editor(typeof(NoEditor), typeof(UITypeEditor))]
//    #endif
//    public class X509RelativeDistinguishedNameSequence : Asn1ReadOnlyCollection<KeyValuePair<Asn1ObjectIdentifier, Object>>, IJsonSerializable, IX509RelativeDistinguishedNameSequence
//        {
//        private class X509RelativeDistinguishedNamePropertyDescriptor : PropertyDescriptor
//            {
//            private Object Value { get; }
//            public X509RelativeDistinguishedNamePropertyDescriptor(Object value, String name, Attribute[] attributes)
//                : base(name, attributes)
//                {
//                Value = value;
//                }

//            #region M:CanResetValue(Object):Boolean
//            /**
//             * <summary>When overridden in a derived class, returns whether resetting an object changes its value.</summary>
//             * <param name="component">The component to test for reset capability.</param>
//             * <returns>true if resetting the component changes its value; otherwise, false.</returns>
//             * */
//            public override Boolean CanResetValue(Object component)
//                {
//                return false;
//                }
//            #endregion
//            #region M:GetValue(Object):Object
//            /**
//             * <summary>When overridden in a derived class, gets the current value of the property on a component.</summary>
//             * <param name="component">The component with the property for which to retrieve the value.</param>
//             * <returns>The value of a property for a given component.</returns>
//             * */
//            public override Object GetValue(Object component)
//                {
//                return Value;
//                }
//            #endregion
//            #region M:ResetValue(Object)
//            /**
//             * <summary>When overridden in a derived class, resets the value for this property of the component to the default value.</summary>
//             * <param name="component">The component with the property value that is to be reset to the default value.</param>
//             * */
//            public override void ResetValue(Object component)
//                {
//                throw new NotImplementedException();
//                }
//            #endregion
//            #region M:SetValue(Object,Object)
//            /**
//             * <summary>When overridden in a derived class, sets the value of the component to a different value.</summary>
//             * <param name="component">The component with the property value that is to be set.</param>
//             * <param name="value">The new value.</param>
//             * */
//            public override void SetValue(Object component, Object value)
//                {
//                throw new NotImplementedException();
//                }
//            #endregion
//            #region M:ShouldSerializeValue(Object):Boolean
//            /**
//             * <summary>When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.</summary>
//             * <param name="component">The component with the property to be examined for persistence.</param>
//             * <returns>true if the property should be persisted; otherwise, false.</returns>
//             * */
//            public override Boolean ShouldSerializeValue(Object component)
//                {
//                return false;
//                }
//            #endregion

//            public override Type ComponentType { get { return typeof(X509RelativeDistinguishedNameSequence); }}
//            public override Boolean IsReadOnly { get { return true; }}
//            public override Type PropertyType  { get { return (Value != null) ? Value.GetType() : typeof(Object); }}
//            public override String DisplayName { get {
//                return Asn1DecodedObjectIdentifierTypeConverter.ToString(new Oid(Name));
//                }}

//            /**
//             * <summary>Returns a string that represents the current object.</summary>
//             * <returns>A string that represents the current object.</returns>
//             * <filterpriority>2</filterpriority>
//             * */
//            public override String ToString()
//                {
//                return Name;
//                }
//            }

//        public X509RelativeDistinguishedNameSequence(IEnumerable<KeyValuePair<Asn1ObjectIdentifier, Object>> source)
//            : base(source)
//            {
//            }

//        public X509RelativeDistinguishedNameSequence()
//            : base(new Dictionary<Asn1ObjectIdentifier, Object>())
//            {
//            }

//        protected override PropertyDescriptorCollection EnsureOverride()
//            {
//            var r = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
//            foreach (var pair in Items) {
//                r.Add(new X509RelativeDistinguishedNamePropertyDescriptor(
//                    pair.Value,
//                    pair.Key.ToString(),
//                    new Attribute[0]));
//                }
//            return r;
//            }

//        #region M:ToString(Object):String
//        internal static String ToString(Object source) {
//            if (source == null) { return String.Empty; }
//            if (source is String) {
//                var value = (String)source;
//                if (value.IndexOf("\"") != -1) {
//                    return $"\"{value.Replace("\"", "\"\"")}\"";
//                    }
//                }
//            return source.ToString();
//            }
//        #endregion
//        #region M:ToString:String
//        /**
//         * <summary>Returns a string that represents the current object.</summary>
//         * <returns>A string that represents the current object.</returns>
//         * <filterpriority>2</filterpriority>
//         * */
//        public override String ToString()
//            {
//            return String.Join(", ", Items.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value)}"));
//            }
//        #endregion
//        #region M:Contains(String,Func<String>):Boolean
//        public Boolean Contains(String key, Func<String, Boolean> comparer) {
//            if (comparer == null) { throw new ArgumentNullException(nameof(comparer)); }
//            foreach (var item in Items) {
//                if (item.Key.Equals(key)) {
//                    return comparer.Invoke(ToString(item.Value));
//                    }
//                }
//            return false;
//            }
//        #endregion
//        public Object this[String key] { get {
//            if (key == null) { throw new ArgumentNullException(nameof(key)); }
//            foreach (var item in Items) {
//                if (item.Key.Equals(key)) { return item.Value; }
//                }
//            throw new ArgumentOutOfRangeException(nameof(key));
//            }}

//        public Boolean TryGetValue(String key, out String r) {
//            r = null;
//            if (key == null) { throw new ArgumentNullException(nameof(key)); }
//            foreach (var item in Items) {
//                if (item.Key.Equals(key)) {
//                    r = item.Value?.ToString();
//                    return true;
//                    }
//                }
//            return false;
//            }

//        public Object this[Asn1ObjectIdentifier key] { get {
//            if (key == null) { throw new ArgumentNullException(nameof(key)); }
//            foreach (var item in Items) {
//                if (item.Key.Equals(key)) { return item.Value; }
//                }
//            throw new ArgumentOutOfRangeException(nameof(key));
//            }}

//        public void WriteJson(JsonWriter writer, JsonSerializer serializer) {
//            using (writer.ObjectScope(serializer)) {
//                writer.WriteValue(serializer, nameof(Count), Count.ToString());
//                writer.WriteValue(serializer, "(Self)", ToString());
//                writer.WritePropertyName("[Self]");
//                using (writer.ArrayScope(serializer)) {
//                    var values = Items.ToArray();
//                    var n = values.Max(i => i.Key.ToString().Length);
//                    var formatting = writer.Formatting;
//                    writer.Formatting = Formatting.None;
//                    for (var i = 0; i < values.Length; i++) {
//                        var type = values[i].Key.ToString();
//                        if (i == 0)
//                            {
//                            //writer.WriteIndent();
//                            }
//                        else
//                            {
//                            writer.Formatting = Formatting.Indented;
//                            }
//                        writer.WriteStartObject();
//                        writer.Formatting = Formatting.None;
//                            writer.WriteValue(serializer, "Type",  type);
//                            //writer.WriteIndentSpace(n - type.Length);
//                            writer.WriteValue(serializer, "Value", values[i].Value.ToString());
//                        writer.WriteEndObject();
//                        }
//                    writer.Formatting = formatting;
//                    }
//                }
//            }

//        Boolean IX509GeneralName.IsEmpty { get {
//            return Count == 0;
//            }}

//        public X509GeneralNameType Type { get { return X509GeneralNameType.Directory; }}
//        }
//    }