using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace shell
    {
    /// <summary>
    /// Interaction logic for EBase64Edit.xaml
    /// </summary>
    public partial class EBase64Edit : UserControl
        {
        public EBase64Edit()
            {
            InitializeComponent();
            }

        #region P:Text:String
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(EBase64Edit), new PropertyMetadata(default(String), OnTextChanged));
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as EBase64Edit);
            if (source != null) {
                source.OnTextChanged();
                }
            }

        private void OnTextChanged()
            {
            try
                {
                Bytes = Convert.FromBase64String(Text);
                }
            catch
                {
                Bytes = null;
                }
            }

        public String Text
            {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
            }
        #endregion
        #region P:Bytes:Byte[]
        private static readonly DependencyPropertyKey BytesPropertyKey = DependencyProperty.RegisterReadOnly("Bytes", typeof(Byte[]), typeof(EBase64Edit), new PropertyMetadata(default(Byte[]), OnBytesChanged));
        public static readonly DependencyProperty BytesProperty = BytesPropertyKey.DependencyProperty;
        private static void OnBytesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as EBase64Edit);
            if (source != null) {
                source.OnBytesChanged();
                }
            }

        private void OnBytesChanged()
            {
            }

        public Byte[] Bytes {
            get { return (Byte[])GetValue(BytesProperty); }
            private set { SetValue(BytesPropertyKey, value); }
            }
        #endregion

        public static RoutedCommand Yes = new RoutedUICommand();
        public static RoutedCommand No  = new RoutedUICommand();
        public static RoutedCommand FormatCommand  = new RoutedUICommand();

        private void SaveAsCanExecuted(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.Handled = true;
            e.CanExecute = (Bytes != null) && (Bytes.Length > 0);
            }

        private void FormatCanExecuted(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.Handled = true;
            e.CanExecute = (Bytes != null) && (Bytes.Length > 0);
            }

        private void YesCanExecuted(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.Handled = true;
            e.CanExecute = (Bytes != null) && (Bytes.Length > 0);
            }

        private void NoCanExecuted(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.Handled = true;
            e.CanExecute = true;
            }

        private void SaveAsExecuted(Object sender, ExecutedRoutedEventArgs e)
            {
            e.Handled = true;
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true) {
                File.WriteAllBytes(dialog.FileName, Bytes);
                }
            }

        private void FormatExecuted(Object sender, ExecutedRoutedEventArgs e)
            {
            e.Handled = true;
            Text = Convert.ToBase64String(Bytes, Base64FormattingOptions.InsertLineBreaks);
            }

        private void YesExecuted(Object sender, ExecutedRoutedEventArgs e)
            {
            e.Handled = true;
            }

        private void NoExecuted(Object sender, ExecutedRoutedEventArgs e)
            {
            e.Handled = true;
            }
        }
    }
