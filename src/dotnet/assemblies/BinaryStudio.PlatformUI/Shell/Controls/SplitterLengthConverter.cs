using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace BinaryStudio.PlatformUI {
    public class SplitterLengthConverter : TypeConverter
        {
        /// <summary>Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.</summary>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from. </param>
        public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            switch (Type.GetTypeCode(sourceType))
                {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                return true;
                }
            return false;
            }

        /// <summary>Returns whether this converter can convert the object to the specified type, using the specified context.</summary>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to. </param>
        public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return !(destinationType != typeof(InstanceDescriptor)) || destinationType == typeof(String);
            }

        /// <summary>Converts the given object to the type of this converter, using the specified context and culture information.</summary>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value) {
            if (value == null || !base.CanConvertFrom(value.GetType()))
                {
                throw base.GetConvertFromException(value);
                }
            if (value is String)
                {
                return SplitterLengthConverter.FromString((String)value, culture);
                }
            Double num = Convert.ToDouble(value, culture);
            if (Double.IsNaN(num))
                {
                num = 1.0;
                }
            return new SplitterLength(num, SplitterUnitType.Stretch);
            }

        /// <summary>Converts the given value object to the specified type, using the specified context and culture information.</summary>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType" /> parameter is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType) {
            if (destinationType == null)
                {
                throw new ArgumentNullException("destinationType");
                }
            if (value != null && value is SplitterLength)
                {
                SplitterLength length = (SplitterLength)value;
                if (destinationType == typeof(String))
                    {
                    return SplitterLengthConverter.ToString(length, culture);
                    }
                if (destinationType.IsEquivalentTo(typeof(InstanceDescriptor)))
                    {
                    return new InstanceDescriptor(typeof(SplitterLength).GetConstructor(new Type[]
                    {
                        typeof(Double),
                        typeof(SplitterUnitType)
                    }), new Object[]
                    {
                        length.Value,
                        length.SplitterUnitType
                    });
                    }
                }
            throw base.GetConvertToException(value, destinationType);
            }

        internal static SplitterLength FromString(String s, CultureInfo cultureInfo) {
            String text = s.Trim();
            Double value = 1.0;
            SplitterUnitType unitType = SplitterUnitType.Stretch;
            if (text == "*")
                {
                unitType = SplitterUnitType.Fill;
                }
            else
                {
                value = Convert.ToDouble(text, cultureInfo);
                }
            return new SplitterLength(value, unitType);
            }

        internal static String ToString(SplitterLength length, CultureInfo cultureInfo) {
            if (length.SplitterUnitType == SplitterUnitType.Fill)
                {
                return "*";
                }
            return Convert.ToString(length.Value, cultureInfo);
            }
        }
    }