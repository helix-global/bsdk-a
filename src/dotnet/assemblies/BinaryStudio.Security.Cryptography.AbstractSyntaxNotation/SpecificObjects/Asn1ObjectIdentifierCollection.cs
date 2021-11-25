using System;
using System.Collections.Generic;
using System.ComponentModel;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1ObjectIdentifierCollection : Asn1ReadOnlyCollection<Asn1ObjectIdentifier>, IJsonSerializable
        {
        private class X509ObjectIdentifierPropertyDescriptor : PropertyDescriptor
            {
            private Asn1ObjectIdentifier Value { get; }
            private X509ObjectIdentifierPropertyDescriptor(Asn1ObjectIdentifier value, String name, Attribute[] attributes)
                : base(name, attributes)
                {
                Value = value;
                }

            public X509ObjectIdentifierPropertyDescriptor(Asn1ObjectIdentifier value, String name)
                : this(value, name, new Attribute[]{ new TypeConverterAttribute(typeof(Asn1DecodedObjectIdentifierTypeConverter)) })
                {
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
                return Value.FriendlyName;
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

            /**
             * <summary>Returns a string that represents the current object.</summary>
             * <returns>A string that represents the current object.</returns>
             * <filterpriority>2</filterpriority>
             * */
            public override String ToString()
                {
                return Value.ToString();
                }
            }

        public Asn1ObjectIdentifierCollection(IEnumerable<Asn1ObjectIdentifier> source)
            : base(source)
            {
            }

        protected override PropertyDescriptorCollection EnsureOverride() {
            var r = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            var i = 0;
            foreach (var item in Items) {
                r.Add(new X509ObjectIdentifierPropertyDescriptor(item, $"[{i}]"));
                i++;
                }
            return r;
            }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartArray();
            foreach (var i in Items) {
                writer.WriteValue(i.ToString());
                }
            writer.WriteEndArray();
            }
        }
    }