using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class MenuItemIconConverter : IValueConverter
        {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            var imagesource = value as ImageSource;
            if (imagesource != null) {
                var result = new Image
                    {
                    Source = imagesource,
                    Width = 16.0,
                    Height = 16.0
                    };
                return result;
                }
            var rc = value as Rectangle;
            if (rc != null) {
                return new Rectangle{
                    Width = 16,
                    Height = 16,
                    Fill = rc.Fill.Clone()
                    };
                }
            return value;
            }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }