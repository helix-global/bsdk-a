using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.PublicKeyInfrastructure;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class X509AlgorithmIdentifierTypeConverter : ObjectTypeConverter
        {
        /// <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or <see langword="null"/> if there are no properties.</returns>
        protected override IEnumerable<PropertyDescriptor> GetPropertiesInternal(ITypeDescriptorContext context, Object value, Attribute[] attributes) {
            var flags = true;
            if (value is X509AlgorithmIdentifier identifier) {
                var parameters = identifier.Parameters;
                //flags = (parameters != null) && (parameters.Count > 0);
                }
            if (flags) {
                foreach (var descriptor in base.GetPropertiesInternal(context, value, attributes)) {
                    yield return descriptor;
                    }
                }
            }

        /// <summary>Returns whether this object supports properties, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns><see langword="true"/> if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, <see langword="false"/>.</returns>
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context) {
            if (context != null) {
                if (context.PropertyDescriptor != null) {
                    var type = context.PropertyDescriptor.PropertyType;
                    if (type.IsValueType) { return false; }
                    if (type == typeof(String)) { return false; }
                    var value = context.PropertyDescriptor.GetValue(context.Instance);
                    return GetPropertiesInternal(context, value, new Attribute[0]).Any();
                    }
                }
            return false;
            }
        }
    }