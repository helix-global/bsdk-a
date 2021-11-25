using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Controls
    {
    using UITextBox = System.Windows.Controls.TextBox;
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

        private static void OnTextWrappingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is UITextBox) {
                ((UITextBox)sender).TextWrapping = (TextWrapping)e.NewValue;
                }
            }
        }
    }