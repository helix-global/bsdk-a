using System;
using System.Globalization;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI
    {
    public class FileSize : IValueConverter
        {
        private static readonly NumberFormatInfo NumberFormat;
        private const Double K = 1024.0;
        private const Double M = 1048576.0;
        private const Double G = 1073741824.0;

        static FileSize()
            {
            NumberFormat = new CultureInfo("en-US", false).NumberFormat;
            NumberFormat.NumberGroupSeparator = " ";
            }

        Object IValueConverter.Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            var i = parameter?.ToString();
            var r = ToDouble(value, culture);
            switch (i)
                {
                case "K" : { r = r / K; return (r >= 1) ? r.ToString("#,# KB", NumberFormat) : "0 KB"; }
                case "M" : return (r / M).ToString("#,# MB", NumberFormat);
                case "G" : return (r / G).ToString("#,# GB", NumberFormat);
                case "B" : return (r / G).ToString("#,# B", NumberFormat);
                }
            return (r < 1)
                ? "0"
                : r.ToString("#,#", NumberFormat);
            }

        private static Double ToDouble(Object value, CultureInfo culture) {
            if (value == null) { return 0.0; }
            if (value is IConvertible) { return ((IConvertible)value).ToDouble(culture); }
            Double r;
            return (Double.TryParse(value.ToString(), out r))
                ? r
                : 0.0;
            }

        Object IValueConverter.ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }