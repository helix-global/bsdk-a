using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Controls
    {
    using UITextBox = System.Windows.Controls.TextBox;
    using ICTextEditor=ICSharpCode.AvalonEdit.TextEditor;

    public class TextBox : UITextBox
        {
        #region P:TextBox.TextWrapping:TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.RegisterAttached("TextWrapping", typeof(TextWrapping), typeof(TextBox), new PropertyMetadata(TextWrapping.NoWrap, OnTextWrappingChanged));
        public static void SetTextWrapping(DependencyObject source, TextWrapping value)
            {
            if (source == null) { throw new ArgumentNullException("source"); }
            source.SetValue(TextWrappingProperty, value);
            }

        public static TextWrapping GetTextWrapping(DependencyObject source)
            {
            if (source == null) { throw new ArgumentNullException("source"); }
            return (TextWrapping)source.GetValue(TextWrappingProperty);
            }
        #endregion
        #region P:TextBox.WatermarkTemplate:DataTemplate
        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.RegisterAttached("WatermarkTemplate", typeof(DataTemplate), typeof(TextBox), new PropertyMetadata(default(DataTemplate)));
        public static void SetWatermarkTemplate(DependencyObject source, DataTemplate value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(WatermarkTemplateProperty, value);
            }

        public static DataTemplate GetWatermarkTemplate(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (DataTemplate)source.GetValue(WatermarkTemplateProperty);
            }
        #endregion
        #region P:TextBox.Watermark:Object
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(Object), typeof(TextBox), new PropertyMetadata(default(Object)));
        public static void SetWatermark(DependencyObject source, Object value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(WatermarkProperty, value);
            }

        public static Object GetWatermark(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Object)source.GetValue(WatermarkProperty);
            }
        #endregion
        #region P:TextBox.TextSource:Object
        public static readonly DependencyProperty TextSourceProperty = DependencyProperty.RegisterAttached("TextSource", typeof(Object), typeof(TextBox), new PropertyMetadata(default(Object), OnTextSourceChanged));
        public static void SetTextSource(DependencyObject source, Object value)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(TextSourceProperty, value);
            }

        public static Object GetTextSource(DependencyObject source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Object)source.GetValue(TextSourceProperty);
            }
        #endregion

        private static void OnTextWrappingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is UITextBox source) {
                source.TextWrapping = (TextWrapping)e.NewValue;
                }
            }

        private static void OnTextSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ICTextEditor source) {
                var value = e.NewValue;
                if (value is Uri uri) {
                    if (uri.Scheme == "file") {
                        source.Load(uri.AbsolutePath);
                        }
                    }
                }
            }
        }
    }