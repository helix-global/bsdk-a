using System;
using System.ComponentModel;
using System.Globalization;
using BinaryStudio.Serialization;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    internal class CmsMessageDigestTypeConverter : TypeConverter
        {
        /// <summary>Returns whether this object supports properties, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns><see langword="true"/> if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, <see langword="false"/>.</returns>
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context)
            {
            return false;
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
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object"/> that represents the converted value.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType" /> parameter is <see langword="null"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
            {
            if (destinationType == typeof(String)) {
                if (value is null) { return "{none}"; }
                if (value is Byte[] bytes) {
                    return $"{bytes.ToString("X")} {{Size = {bytes.Length}}}";
                    }
                }
            return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }