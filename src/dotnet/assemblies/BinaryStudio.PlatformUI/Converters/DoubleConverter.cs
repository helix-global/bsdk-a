using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI
    {
    public class DoubleConverter : IValueConverter {
        private static readonly CultureInfo EN = CultureInfo.GetCultureInfo("en-US");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static Double? Convert(Object value, CultureInfo culture) {
            if ((value == null) || (value is DBNull)) { return null; }
            if (value is IConvertible) {
                return ((IConvertible)value).ToDouble(culture);
                }
            return null;
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Double? Convert(String value) {
            if (value == null) { return null; }
            try
                {
                return Double.Parse(((String)value).Replace(",", "."), EN);
                }
            catch (Exception)
                {
                return null;
                }
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            var r = Convert(value, culture);
            if (r != null) {
                if (parameter is String) {
                    switch ((String)parameter) {
                        case "*(-1)" : { return -r; }
                        default:
                            {
                            var regex = new Regex(@"^[*][(](\d+[.]\d+)[)]$");
                            if (regex.IsMatch((String)parameter)) {
                                var multiplicator = ((String)parameter).Substring(2, ((String)parameter).Length - 3);
                                return r*(Convert(multiplicator)).GetValueOrDefault(1.0);
                                }
                            }
                            break;
                        }
                    }
                }
            return r;
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            if ((value is String) && (targetType == typeof(Double))) {
                try
                    {
                    return Double.Parse(((String)value).Replace(",", "."));
                    }
                catch (Exception)
                    {
                    return value;
                    }
                }
            throw new NotImplementedException();
            }
        }
    }