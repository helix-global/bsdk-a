using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters
    {
    internal class Asn1ObjectConverter : TypeConverter
        {
        /**
         * <summary>Returns whether this object supports properties, using the specified context.</summary>
         * <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
         * <returns>true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, false.</returns>
         * */
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context) {
            if (context != null) {
                var descriptor = context.PropertyDescriptor;
                if (descriptor != null) {
                    var value = descriptor.GetValue(context.Instance) as Asn1Object;
                    if (value != null) {
                        return value.Count > 0;
                        }
                    }
                }
            return false;
            }

        private class ArrayPropertyDescriptor : PropertyDescriptor
            {
            private Int32 Index { get; }
            private IList<Asn1Object> Instance { get; }
            public ArrayPropertyDescriptor(IList<Asn1Object> instance, Int32 index, Attribute[] attributes)
                : base($"[{index}]", attributes)
                {
                Index = index;
                Instance = instance;
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
                return Instance[Index];
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

            public override Type ComponentType { get { return typeof(Object); }}
            public override Boolean IsReadOnly { get { return true; }}
            public override Type PropertyType  { get {
                var value = Instance[Index];
                return (value != null) ? value.GetType() : typeof(Object);
                }}

            /**
             * <summary>Returns a string that represents the current object.</summary>
             * <returns>A string that represents the current object.</returns>
             * <filterpriority>2</filterpriority>
             * */
            public override String ToString()
                {
                var value = Instance[Index];
                return value.ToString();
                }
            }

        /**
         * <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
         * <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
         * <param name="value">An <see cref="Object"/> that specifies the type of array for which to get properties.</param>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter.</param>
         * <returns>A <see cref="PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or null if there are no properties.</returns>
         * */
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, Object value, Attribute[] attributes)
            {
            var source = value as Asn1Object;
            if ((source != null) && (source.Count > 0)) {
                var r = new List<PropertyDescriptor>();
                var i = 0;
                foreach (var item in source) {
                    r.Add(new ArrayPropertyDescriptor(source, i, new Attribute[0]));
                    i++;
                    }
                return new PropertyDescriptorCollection(r.ToArray());
                }
            return base.GetProperties(context, value, attributes);
            }
        }
    }