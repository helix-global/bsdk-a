using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    internal class X509ByteArrayTypeConverter : TypeConverter
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
        /// <returns><see langword="true"/>if this converter can perform the conversion; otherwise, <see langword="false"/>.</returns>
        public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
            return (destinationType == typeof(String)) ||
                base.CanConvertTo(context, destinationType);
            }

        /// <summary>Converts the given value object to the specified type, using the specified context and culture information.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If <see langword="null"/> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>An <see cref="T:System.Object"/> that represents the converted value.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
            {
            if (destinationType == typeof(String)) {
                if (value == null) { return "{none}"; }
                if (value is Byte[] bytes) {
                    if (bytes.Length == 0) { return "{empty}"; }
                    if (bytes.Length > 40) { return $"{{Size = {bytes.Length}}}"; }
                    return String.Join(String.Empty, bytes.
                        Select(i => i.ToString("X2")));
                    }
                if (value is BigInteger bigint) {
                    return String.Join(String.Empty, bigint.
                        ToByteArray().
                        Reverse().
                        Select(i => i.ToString("X2")));
                    }
                if (value is Asn1OctetString octet) {
                    return ConvertTo(context, culture,
                        octet.Content.ToArray(),
                        destinationType);
                    }
                if (value is Asn1BitString bitstring) {
                    return ConvertTo(context, culture,
                        bitstring.Content.ToArray(),
                        destinationType);
                    }
                }
            return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }