using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
#if FEATURE_WPF_VALUE_CONVERTER
using System.Windows.Data;
#endif

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters
    {
    internal class X509CertificateKeyUsageTypeConverter : TypeConverter
        #if FEATURE_WPF_VALUE_CONVERTER
        , IValueConverter
        #endif
        {
        public static String ToString(X509KeyUsageFlags value, CultureInfo culture) {
            return String.Join(",", ToStringArray(value, culture));
            }

        public static IList<String> ToStringArray(X509KeyUsageFlags value, CultureInfo culture) {
            var r = new List<String>();
            var n = (UInt32)value;
            if ((n & (UInt32)X509KeyUsageFlags.EncipherOnly)     != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.EncipherOnly),     culture)); n &= ~(UInt32)X509KeyUsageFlags.EncipherOnly;     }
            if ((n & (UInt32)X509KeyUsageFlags.CrlSign)          != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.CrlSign),          culture)); n &= ~(UInt32)X509KeyUsageFlags.CrlSign;          }
            if ((n & (UInt32)X509KeyUsageFlags.KeyCertSign)      != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.KeyCertSign),      culture)); n &= ~(UInt32)X509KeyUsageFlags.KeyCertSign;      }
            if ((n & (UInt32)X509KeyUsageFlags.KeyAgreement)     != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.KeyAgreement),     culture)); n &= ~(UInt32)X509KeyUsageFlags.KeyAgreement;     }
            if ((n & (UInt32)X509KeyUsageFlags.DataEncipherment) != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.DataEncipherment), culture)); n &= ~(UInt32)X509KeyUsageFlags.DataEncipherment; }
            if ((n & (UInt32)X509KeyUsageFlags.KeyEncipherment)  != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.KeyEncipherment),  culture)); n &= ~(UInt32)X509KeyUsageFlags.KeyEncipherment;  }
            if ((n & (UInt32)X509KeyUsageFlags.NonRepudiation)   != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.NonRepudiation),   culture)); n &= ~(UInt32)X509KeyUsageFlags.NonRepudiation;   }
            if ((n & (UInt32)X509KeyUsageFlags.DigitalSignature) != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.DigitalSignature), culture)); n &= ~(UInt32)X509KeyUsageFlags.DigitalSignature; }
            if ((n & (UInt32)X509KeyUsageFlags.DecipherOnly)     != 0) { r.Add(Resources.ResourceManager.GetString(nameof(X509KeyUsageFlags) + "." + nameof(X509KeyUsageFlags.DecipherOnly),     culture)); n &= ~(UInt32)X509KeyUsageFlags.DecipherOnly;     }
            return r;
            }

        public static String ToString(X509KeyUsageFlags value) {
            return ToString(value, PlatformSettings.DefaultCulture);
            }

        public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
            if (destinationType == typeof(String))      { return true; }
            if (destinationType == typeof(IEnumerable)) { return true; }
            return base.CanConvertTo(context, destinationType);
            }

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
                if (value is X509KeyUsageFlags) {
                    if (destinationType == typeof(IEnumerable)) { return ToStringArray((X509KeyUsageFlags)value, culture); }
                    if (destinationType == typeof(String))      { return ToString((X509KeyUsageFlags)value, culture);      }
                    if (destinationType == typeof(Object))      { return value;                                            }
                    }
                }
            return value;
            }
        #endregion
        #if FEATURE_WPF_VALUE_CONVERTER
        #region M:IValueConverter.Convert(Object,Type,Object,CultureInfo):Object
        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            if (value == null) { return null; }
            return CanConvertTo(targetType)
                ? ConvertTo(null, PlatformSettings.DefaultCulture, value, targetType)
                : ConvertTo(null, PlatformSettings.DefaultCulture, value, typeof(Object));
            }
        #endregion
        #region M:IValueConverter.ConvertBack(Object,Type,Object,CultureInfo):Object
        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            return value;
            }
        #endregion
        #endif
        }
    }