using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    internal class CmsMessageTypeConverter : TypeConverter
        {
        /// <summary>Returns whether this object supports properties, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns> <see langword="true"/> if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, <see langword="false"/>.</returns>
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context)
            {
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

        /// <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or <see langword="null"/> if there are no properties.</returns>
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
            return base.GetProperties(context, value, attributes);
            }
        }
    }