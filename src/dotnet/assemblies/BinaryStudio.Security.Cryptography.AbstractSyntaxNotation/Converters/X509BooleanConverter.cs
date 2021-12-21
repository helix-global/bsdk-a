using System;
using System.ComponentModel;
using System.Globalization;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
#if FEATURE_WPF_VALUE_CONVERTER
using System.Windows.Data;
#endif

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters
    {
    internal class X509BooleanConverter : TypeConverter
        #if FEATURE_WPF_VALUE_CONVERTER
        , IValueConverter
        #endif
        {
        #region M:GetPropertiesSupported(ITypeDescriptorContext):Boolean
        /**
         * <summary>Returns whether this object supports properties, using the specified context.</summary>
         * <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
         * <returns>true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, false.</returns>
         * */
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context)
            {
            return false;
            }
        #endregion
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
            if (value is Boolean) {
                if (destinationType == typeof(String)) {
                    return ToString((Boolean)value, culture);
                    }
                }
            return base.ConvertTo(context, culture, value, destinationType);
            }
        #endregion

        public static String ToString(Boolean value, CultureInfo culture) {
            return value
                ? Resources.ResourceManager.GetString(nameof(Boolean) + ".True",  culture)
                : Resources.ResourceManager.GetString(nameof(Boolean) + ".False", culture);
            }

        public static String ToString(Boolean value) {
            return ToString(value, PlatformContext.DefaultCulture);
            }

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