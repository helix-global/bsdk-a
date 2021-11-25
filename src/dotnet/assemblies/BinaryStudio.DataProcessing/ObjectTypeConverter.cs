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
        #region M:CanConvertFrom(ITypeDescriptorContext,Type):Boolean
        public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return (sourceType == typeof(String)) || base.CanConvertFrom(context, sourceType);
            }
        #endregion
        #region M:CanConvertTo(ITypeDescriptorContext,Type):Boolean
        public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
            return base.CanConvertTo(context, destinationType);
            }
        #endregion
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
        #region M:ConvertTo(ITypeDescriptorContext,CultureInfo,Object,Type):Object
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
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
        #endregion
        #region M:GetProperties(ITypeDescriptorContext,Object,Attribute[]):PropertyDescriptorCollection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, Object value, Attribute[] attributes) {
            if (value != null) {
                var descriptors = TypeDescriptor.GetProperties(value, attributes).OfType<PropertyDescriptor>().ToArray();
                var r = new List<PropertyDescriptor>();
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
                            r.Add(descriptors[i]);
                            }
                        }
                    else
                        {
                        r.Add(descriptors[i]);
                        }
                    }
                foreach (var service in services) {
                    var descriptor = new CompositePropertyDescriptor(
                        value,
                        service.Key, new Attribute[] {
                            new TypeConverterAttribute(GetType())
                            }, service.Value);
                    r.Add(descriptor);
                    }
                return new PropertyDescriptorCollection(r.ToArray());
                }
            
            //if (value is IList) {
            //    var r = new PropertyDescriptor[((IList)value).Count];
            //    var type = value.GetType();
            //    for (var i = 0; i < ((IList)value).Count; i++) {
            //        var index = i;
            //        r[i] = new ArrayPropertyDescriptor(
            //            (IList)value,
            //            i,
            //            new Attribute[] {
            //                new TypeConverterAttribute(GetType())
            //                }
            //            );
            //        }
            //    return new PropertyDescriptorCollection(r);
            //    }
            return base.GetProperties(context, value, attributes);
            }
        #endregion
        #region M:GetPropertiesSupported(ITypeDescriptorContext):Boolean
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context) {
            if (context != null) {
                if (context.PropertyDescriptor != null) {
                    var type = context.PropertyDescriptor.PropertyType;
                    if (type.IsValueType) { return false; }
                    if (type == typeof(String)) { return false; }
                    var value = context.PropertyDescriptor.GetValue(context.Instance);
                    if (value == null) { return true; }
                    if ((value is IList) && (((IList)value).Count == 0)) { return false; }
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
                if ((value is IList) && (((IList)value).Count == 0)) { return "(empty)"; }
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
                }
            return (value != null)
                ? value.ToString()
                : (type == typeof(String))
                    ? String.Empty
                    : "(none)";
            }
        #endregion
        #region M:ToString(Boolean):String
        protected virtual String ToString(Boolean source) {
            return (Boolean)source
                ? Resources.ObjectTypeConverterTrue
                : Resources.ObjectTypeConverterFalse;
            }
        #endregion
        }
    }