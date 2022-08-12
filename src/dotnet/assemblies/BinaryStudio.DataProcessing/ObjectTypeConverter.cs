using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using BinaryStudio.DataProcessing.Properties;

namespace BinaryStudio.DataProcessing
    {
    public class ObjectTypeConverter : TypeConverter
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

        #region M:ConvertFrom(ITypeDescriptorContext,CultureInfo,Object):Object
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value) {
            if ((context != null) && (context.PropertyDescriptor != null)) {
                var type = context.PropertyDescriptor.PropertyType;
                if (type.IsEnum && (value is String)) {
                    return Enum.Parse(type, (String)value, true);
                    }
                }
            return base.ConvertFrom(context, culture, value);
            }
        #endregion

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
                    if (value is Boolean) { return ToString((Boolean)value); }
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

        /// <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or <see langword="null"/> if there are no properties.</returns>
        protected virtual IEnumerable<PropertyDescriptor> GetPropertiesInternal(ITypeDescriptorContext context, Object value, Attribute[] attributes) {
            if (value != null) {
                var descriptors = TypeDescriptor.GetProperties(value, attributes).OfType<PropertyDescriptor>().ToArray();
                var services = new SortedDictionary<String, IList<PropertyDescriptor>>();
                for (var i= 0; i < descriptors.Length; i++) {
                    var displayname = descriptors[i].DisplayName ?? descriptors[i].Name;
                    var index = displayname.IndexOf(".", StringComparison.Ordinal);
                    if (index != -1) {
                        var service = displayname.Substring(0, index);
                        if (Char.IsLetter(service[0])) {
                            IList<PropertyDescriptor> list;
                            if (!services.TryGetValue(service, out list)) { services.Add(service, list = new List<PropertyDescriptor>()); }
                            list.Add(descriptors[i]);
                            }
                        else
                            {
                            yield return descriptors[i];
                            }
                        }
                    else
                        {
                        yield return descriptors[i];
                        }
                    }
                foreach (var service in services) {
                    var descriptor = new CompositePropertyDescriptor(
                        value,
                        service.Key, new Attribute[] {
                            new TypeConverterAttribute(GetType())
                            }, service.Value);
                    yield return descriptor;
                    }
                }
            else
                {
                foreach (PropertyDescriptor descriptor in base.GetProperties(context, value, attributes))
                    {
                    yield return descriptor;
                    }
                }
            }

        /// <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or <see langword="null"/> if there are no properties.</returns>
        public sealed override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, Object value, Attribute[] attributes) {
            var r = GetPropertiesInternal(
                    context,
                    value,
                    attributes).
                OrderBy(i => i, DefaultComparer).
                ToArray();
            return new PropertyDescriptorCollection(r);
            }

        #region M:GetPropertiesSupported(ITypeDescriptorContext):Boolean
        /// <summary>Returns whether this object supports properties, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns><see langword="true"/> if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, <see langword="false"/>.</returns>
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context) {
            if (context != null) {
                if (context.PropertyDescriptor != null) {
                    var type = context.PropertyDescriptor.PropertyType;
                    if (type.IsValueType) { return false; }
                    if (type == typeof(String)) { return false; }
                    //var value = context.PropertyDescriptor.GetValue(context.Instance);
                    //if (value == null) { return true; }
                    //if ((value is IList) && (((IList)value).Count == 0)) { return false; }
                    }
                }
            return true;
            }
        #endregion
        #region M:GetStandardValuesExclusive(ITypeDescriptorContext):Boolean
        public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
            return base.GetStandardValuesExclusive(context);
            }
        #endregion
        #region M:GetStandardValuesSupported(ITypeDescriptorContext):Boolean
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context) {
            if ((context != null) && (context.PropertyDescriptor != null)) {
                if (context.PropertyDescriptor.PropertyType.IsEnum) { return true; }
                }
            return base.GetStandardValuesSupported(context);
            }
        #endregion
        #region M:GetElementType(Type):Type
        internal static Type GetElementType(Type source) {
            var r = source.GetElementType();
            if (r == null) {
                if (source.IsGenericType) {
                    var type = source.GetInterfaces().FirstOrDefault(i => (i.IsGenericType) && (i.Namespace=="System.Collections.Generic") && (i.Name=="IEnumerable`1"));
                    if (type != null) { r = type.GetGenericArguments()[0]; }
                    }
                }
            return r;
            }
        #endregion
        #region M:ToString(Object,Type,CultureInfo):String
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected virtual String ToString(Object value, Type type, CultureInfo culture) {
            if (value != null) {
                if ((value is IList) && (((IList)value).Count == 0)) { return "{empty}"; }
                if (value is IList) { return Resources.CollectionConverterText; }
                if (value is ICollection) { return Resources.CollectionConverterText; }
                if (value is Boolean) { return ToString((Boolean)value); }
                if ((type != null) && (type.IsEnum)) {
                    var utype = Enum.GetUnderlyingType(type);
                    if ((value is IConvertible) && (value.GetType() != utype)) {
                        value = ((IConvertible)value).ToType(utype, culture);
                        }
                    return Enum.Format(type, value, "G");
                    }
                var descriptor = TypeDescriptor.GetDefaultProperty(value);
                if (descriptor != null) {
                    var r = descriptor.GetValue(GetPropertyOwner(value, descriptor));
                    if (r != null) {
                        var converter = descriptor.Converter
                            ?? TypeDescriptor.GetConverter(r)
                            ?? TypeDescriptor.GetConverter(r.GetType());
                        return (converter.CanConvertTo(typeof(String)))
                            ? converter.ConvertToString(r)
                            : ToString(r, r.GetType(), culture);
                        }
                    }
                }
            return (value != null)
                ? value.ToString()
                : (type == typeof(String))
                    ? String.Empty
                    : "{none}";
            }
        #endregion
        #region M:ToString(Boolean):String
        protected virtual String ToString(Boolean source) {
            return (Boolean)source
                ? Resources.ObjectTypeConverterTrue
                : Resources.ObjectTypeConverterFalse;
            }
        #endregion

        private static Object GetPropertyOwner(Object source, PropertyDescriptor descriptor) {
            return (source is ICustomTypeDescriptor service)
                ? service.GetPropertyOwner(descriptor)
                : source;
            }

        private static readonly IComparer<PropertyDescriptor> DefaultComparer = new PropertyDescriptorComparer();
        private class PropertyDescriptorComparer : IComparer<PropertyDescriptor>
            {
            /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
            public Int32 Compare(PropertyDescriptor x, PropertyDescriptor y)
                {
                if (ReferenceEquals(x, y))    { return 0;  }
                if (ReferenceEquals(x, null)) { return -1; }
                if (x is IComparable<PropertyDescriptor> e) { return e.CompareTo(y); }
                if (y is null) { return +1; }
                var orderX = (x.Attributes.OfType<OrderAttribute>().FirstOrDefault()?.Order).GetValueOrDefault(0);
                var orderY = (y.Attributes.OfType<OrderAttribute>().FirstOrDefault()?.Order).GetValueOrDefault(0);
                var nameX = x.DisplayName;
                var nameY = y.DisplayName;
                return (orderX == orderY)
                    ? nameX.CompareTo(nameY)
                    : orderX.CompareTo(orderY);
                }
            }
        }
    }