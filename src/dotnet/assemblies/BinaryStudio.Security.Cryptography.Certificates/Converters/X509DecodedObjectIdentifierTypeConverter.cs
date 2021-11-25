using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;

namespace BinaryStudio.Security.Cryptography.Certificates.Converters
    {
    internal class X509DecodedObjectIdentifierTypeConverter : TypeConverter
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
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
            {
            #region Source:Asn1ObjectIdentifier
            if (value is Asn1ObjectIdentifier) {
                var oid = value.ToString();
                var r = OID.ResourceManager.GetString(oid, culture);
                if (String.IsNullOrEmpty(r)) {
                    r = (new Oid(oid)).FriendlyName;
                    }
                return String.IsNullOrEmpty(r)
                    ? oid
                    : r;
                }
            #endregion
            #region Source:Oid
            if (value is Oid) {
                var oid = value.ToString();
                var r = OID.ResourceManager.GetString(oid, culture);
                if (String.IsNullOrEmpty(r)) {
                    r = ((Oid)(value)).FriendlyName;
                    }
                return String.IsNullOrEmpty(r)
                    ? oid
                    : r;
                }
            #endregion
            if (value is IServiceProvider) {
                var r = ((IServiceProvider)value).GetService(typeof(Asn1ObjectIdentifier));
                if (r != null) {
                    return ConvertTo(context, culture, r, destinationType);
                    }
                }
            return base.ConvertTo(context, culture, value, destinationType);
            }
        #endregion
        #region M:ToString(X509ObjectIdentifier):String
        public static String ToString(X509ObjectIdentifier source) {
            var r = source.GetService(typeof(Asn1ObjectIdentifier));
            if (r != null) {
                return ToString((Asn1ObjectIdentifier)r);
                }
            return null;
            }
        #endregion
        #region M:ToString(Asn1ObjectIdentifier):String
        public static String ToString(Asn1ObjectIdentifier value) {
            var oid = value.ToString();
            var r = OID.ResourceManager.GetString(oid, PlatformSettings.DefaultCulture);
            if (String.IsNullOrEmpty(r)) {
                r = (new Oid(oid)).FriendlyName;
                }
            return String.IsNullOrEmpty(r)
                ? oid
                : r;
            }
        #endregion
        #region M:ToString(Oid):String
        public static String ToString(Oid value) {
            var oid = value.Value;
            var r = OID.ResourceManager.GetString(oid, PlatformSettings.DefaultCulture);
            if (String.IsNullOrEmpty(r)) {
                r = (value).FriendlyName;
                }
            return String.IsNullOrEmpty(r)
                ? oid
                : r;
            }
        #endregion
        }
    }