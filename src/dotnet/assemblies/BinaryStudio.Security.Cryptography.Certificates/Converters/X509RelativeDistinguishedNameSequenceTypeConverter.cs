using System;
using System.ComponentModel;
using System.Globalization;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.Security.Cryptography.Certificates.Converters
    {
    internal class X509RelativeDistinguishedNameSequenceTypeConverter : ObjectTypeConverter
        {
        #region M:ConvertTo(ITypeDescriptorContext,CultureInfo,Object,Type):Object
        /**
         * <summary>Converts the given value object to the specified type, using the specified context and culture information.</summary>
         * <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
         * <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.</param>
         * <param name="value">The <see cref="Object"/> to convert.</param>
         * <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.</param>
         * <returns>An <see cref="Object"/> that represents the converted value.</returns>
         * <exception cref="ArgumentNullException">The <paramref name="destinationType"/> parameter is null.</exception>
         * <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
         * */
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType) {
            if (value != null) {
                if (value.GetType() == destinationType) { return value; }
                if (destinationType == typeof(String)) {
                    if (value is X509RelativeDistinguishedNameSequence) { return value.ToString(); }
                    }
                }
            return base.ConvertTo(context, culture, value, destinationType);
            }
        #endregion
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
        }
    }