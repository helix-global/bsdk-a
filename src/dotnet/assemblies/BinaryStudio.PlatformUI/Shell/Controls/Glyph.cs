using System;
using System.Windows;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class Glyph {
        #region P:Glyph.Foreground:Brush
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(Glyph), new PropertyMetadata(default(Brush)));
        public static void SetForeground(DependencyObject source, Brush value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(ForegroundProperty, value);
            }
        public static Brush GetForeground(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Brush)source.GetValue(ForegroundProperty);
            }
        #endregion
        #region P:Glyph.Geometry:Geometry
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.RegisterAttached("Geometry", typeof(Geometry), typeof(Glyph), new PropertyMetadata(default(Geometry)));
        public static void SetGeometry(DependencyObject source, Geometry value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(GeometryProperty, value);
            }

        public static Geometry GetGeometry(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Geometry)source.GetValue(GeometryProperty);
            }
        #endregion
        #region P:Glyph.Stretch:Stretch
        public static readonly DependencyProperty StretchProperty = DependencyProperty.RegisterAttached("Stretch", typeof(Stretch), typeof(Glyph), new PropertyMetadata(default(Stretch)));
        public static void SetStretch(DependencyObject source, Stretch value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(StretchProperty, value);
            }

        public static Stretch GetStretch(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Stretch)source.GetValue(StretchProperty);
            }
        #endregion
        #region P:Glyph.Width:Double
        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width", typeof(Double), typeof(Glyph), new PropertyMetadata(default(Double)));
        public static void SetWidth(DependencyObject source, Double value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(WidthProperty, value);
            }

        public static Double GetWidth(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Double)source.GetValue(WidthProperty);
            }
        #endregion
        #region P:Glyph.Height:Double
        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached("Height", typeof(Double), typeof(Glyph), new PropertyMetadata(default(Double)));
        public static void SetHeight(DependencyObject source, Double value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(HeightProperty, value);
            }

        public static Double GetHeight(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Double)source.GetValue(HeightProperty);
            }
        #endregion
        }
    }