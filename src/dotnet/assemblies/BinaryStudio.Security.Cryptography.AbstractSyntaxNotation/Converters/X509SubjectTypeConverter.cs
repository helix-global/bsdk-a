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
    public class X509SubjectTypeConverter : TypeConverter
        #if FEATURE_WPF_VALUE_CONVERTER
        , IValueConverter
        #endif
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
            if (value is X509SubjectType) {
                if (destinationType == typeof(String)) {
                    return ToString((X509SubjectType)value, PlatformSettings.DefaultCulture);
                    }
                }
            return value;
            }
        #endregion

        public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
            if (destinationType == typeof(String)) { return true; }
            return base.CanConvertTo(context, destinationType);
            }

        public static String ToString(X509SubjectType value, CultureInfo culture) {
            switch (value)
                {
                case X509SubjectType.CA:        { return Resources.ResourceManager.GetString(nameof(X509SubjectType) + "." + nameof(X509SubjectType.CA),        culture); }
                case X509SubjectType.EndEntity: { return Resources.ResourceManager.GetString(nameof(X509SubjectType) + "." + nameof(X509SubjectType.EndEntity), culture); }
                default : { return value.ToString(); }
                }
            }

        public static String ToString(X509SubjectType value) {
            return ToString(value, PlatformSettings.DefaultCulture);
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