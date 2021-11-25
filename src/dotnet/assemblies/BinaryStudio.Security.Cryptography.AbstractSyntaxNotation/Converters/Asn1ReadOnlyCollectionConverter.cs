using System;
using System.ComponentModel;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters
    {
    internal class Asn1ReadOnlyCollectionConverter : TypeConverter
        {
        #region M:GetPropertiesSupported(ITypeDescriptorContext):Boolean
        /**
         * <summary>Returns whether this object supports properties, using the specified context.</summary>
         * <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
         * <returns>true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, false.</returns>
         * */
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context)
            {
            return true;
            }
        #endregion
        #region M:GetProperties(ITypeDescriptorContext,Object,Attribute[]):PropertyDescriptorCollection
        /**
         * <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
         * <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
         * <param name="value">An <see cref="Object"/> that specifies the type of array for which to get properties.</param>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter.</param>
         * <returns>A <see cref="PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or null if there are no properties.</returns>
         * */
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, Object value, Attribute[] attributes)
            {
            return TypeDescriptor.GetProperties(value, attributes);
            }
        #endregion
        }
    }
