using System;
using System.Windows;
using System.Windows.Documents;

namespace BinaryStudio.PlatformUI.Controls
    {
    using UIRichTextBox = System.Windows.Controls.RichTextBox;
    public class RichTextBox : UIRichTextBox
        {
        #region P:RichTextBox.Document:FlowDocument
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.RegisterAttached("Document", typeof(FlowDocument), typeof(RichTextBox), new PropertyMetadata(default(FlowDocument), OnDocumentChanged));
        private static void OnDocumentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = sender as UIRichTextBox;
            if (source != null) {
                var value = (FlowDocument)e.NewValue;
                source.Document = (value != null)
                    ? value
                    : new FlowDocument();
                }
            }

        public static void SetDocument(DependencyObject source, FlowDocument value)
            {
            if (source == null) { throw new ArgumentNullException("source"); }
            source.SetValue(DocumentProperty, value);
            }

        public static FlowDocument GetDocument(DependencyObject source)
            {
            if (source == null) { throw new ArgumentNullException("source"); }
            return (FlowDocument)source.GetValue(DocumentProperty);
            }
        #endregion
        }
    }