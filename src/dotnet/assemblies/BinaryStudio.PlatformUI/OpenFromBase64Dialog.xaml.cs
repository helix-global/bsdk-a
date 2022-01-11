using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Shell.Controls;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI
    {
    /// <summary>
    /// Interaction logic for OpenFromBase64Dialog.xaml
    /// </summary>
    public partial class OpenFromBase64Dialog : Window
        {
        public OpenFromBase64Dialog()
            {
            InitializeComponent();
            }

        #region P:Text:String
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(OpenFromBase64Dialog), new PropertyMetadata(default(String), OnTextChanged));
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as OpenFromBase64Dialog);
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
        private static readonly DependencyPropertyKey BytesPropertyKey = DependencyProperty.RegisterReadOnly("Bytes", typeof(Byte[]), typeof(OpenFromBase64Dialog), new PropertyMetadata(default(Byte[]), OnBytesChanged));
        public static readonly DependencyProperty BytesProperty = BytesPropertyKey.DependencyProperty;
        private static void OnBytesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as OpenFromBase64Dialog);
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
            DialogResult = true;
            }

        private void NoExecuted(Object sender, ExecutedRoutedEventArgs e)
            {
            e.Handled = true;
            DialogResult = false;
            }
        }
    }
