using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace BinaryStudio.DataProcessing
    {
    public class ObjectCollectionTypeConverter : ObjectTypeConverter
        {
        /// <summary>Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns><see langword="true"/> if this converter can perform the conversion; otherwise, <see langword="false"/>.</returns>
        public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return (sourceType == typeof(String)) || base.CanConvertFrom(context, sourceType);
            }

        /// <summary>Returns whether this converter can convert the object to the specified type, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns><see langword="true"/> if this converter can perform the conversion; otherwise, <see langword="false"/>.</returns>
        public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
            if (destinationType == typeof(String)) { return true; }
            return base.CanConvertTo(context, destinationType);
            }

        /// <summary>Converts the given value object to the specified type, using the specified context and culture information.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If <see langword="null"/> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>An <see cref="T:System.Object"/> that represents the converted value.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType) {
            if (value != null) {
                if (value.GetType() == destinationType) { return value; }
                if (destinationType == typeof(String)) {
                    return ToString(value, value.GetType(), culture);
                    }
                }
            else if (destinationType == typeof(String)) {
                return ToString(null, ((context != null) && (context.PropertyDescriptor != null))
                    ? context.PropertyDescriptor.PropertyType
                    : null,
                    culture);
                }
            return base.ConvertTo(context, culture, value, destinationType);
            }

        /// <summary>Returns whether this object supports properties, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns><see langword="true"/> if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, <see langword="false"/>.</returns>
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context) {
            if (context != null) {
                if (context.PropertyDescriptor != null) {
                    var value = context.PropertyDescriptor.GetValue(context.Instance);
                    if (value == null) { return true; }
                    if ((value is IList) && (((IList)value).Count == 0)) { return false; }
                    }
                }
            return true;
            }

        /// <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or <see langword="null"/> if there are no properties.</returns>
        protected override IEnumerable<PropertyDescriptor> GetPropertiesInternal(ITypeDescriptorContext context, Object value, Attribute[] attributes) {
            if (value is IEnumerable items) {
                var source = new List<Object>();
                var i = 0;
                foreach (var item in items) {
                    source.Add(item);
                    yield return new ArrayPropertyDescriptor(value.GetType(), source, i, new Attribute[] {});
                    i++;
                    }
                }
            }

        protected override String ToString(Object value, Type type, CultureInfo culture) {
            if (value != null) {
                type = type ?? value.GetType();
                if (type.IsGenericType && !type.IsGenericTypeDefinition) {
                    var stype = typeof(ICollection<>);
                    var ntype = type.GetInterfaces().FirstOrDefault(i => {
                        if (i.IsGenericType && !i.IsGenericTypeDefinition) {
                            var gtype = i.GetGenericTypeDefinition();
                            if (gtype.Name == stype.Name) {
                                return true;
                                }
                            }
                        return false;
                        });
                    if (ntype != null) {
                        var count = (Int32)ntype.GetProperty("Count").GetValue(value, null);
                        return (count == 0)
                            ? "{empty}"
                            : $"Count = {count}";
                        }
                    }
                }
            return base.ToString(value, type, culture);
            }

        protected class ArrayPropertyDescriptor : PropertyDescriptor
            {
            private Int32 Index { get; }
            private IList<Object> Source { get; }
            public ArrayPropertyDescriptor(Type componentType, IList<Object> source, Int32 index, Attribute[] attributes)
                : base($"[{index}]", attributes)
                {
                ComponentType = componentType;
                Index = index;
                Source = source;
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
                return Source[Index];
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

            public override Type ComponentType { get; }
            public override Boolean IsReadOnly { get { return true; }}
            public override Type PropertyType  { get {
                var value = Source[Index];
                return (value != null)
                    ? value.GetType()
                    : typeof(Object);
                }}

            /**
             * <summary>Returns a string that represents the current object.</summary>
             * <returns>A string that represents the current object.</returns>
             * <filterpriority>2</filterpriority>
             * */
            public override String ToString()
                {
                var value = Source[Index];
                return value.ToString();
                }

            /// <summary>Gets the type converter for this property.</summary>
            /// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> that is used to convert the <see cref="T:System.Type" /> of this property.</returns>
            public override TypeConverter Converter { get {
                var r = base.Converter;
                return r;
                }}
            }
        }
    }