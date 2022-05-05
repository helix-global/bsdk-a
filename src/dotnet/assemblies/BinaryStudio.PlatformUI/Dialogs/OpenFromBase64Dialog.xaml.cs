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
    public partial class OpenFromBase64Dialog : ToolWindow
        {
        public OpenFromBase64Dialog()
            {
            InitializeComponent();
            }

        #region P:Text:String
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(OpenFromBase64Dialog), new PropertyMetadata(default(String), OnTextChanged));
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is OpenFromBase64Dialog source) {
                source.OnTextChanged();
                }
            }

        private void OnTextChanged()
            {
            try
                {
                OutputBytes = Convert.FromBase64String(Text);
                ErrorPanelMessage = String.Empty;
                ErrorPanelVisibility = Visibility.Hidden;
                }
            catch(Exception e)
                {
                ErrorPanelMessage = e.Message;
                ErrorPanelVisibility = Visibility.Visible;
                OutputBytes = null;
                }
            }

        public String Text
            {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
            }
        #endregion
        #region P:OutputBytes:Byte[]
        private static readonly DependencyPropertyKey OutputBytesPropertyKey = DependencyProperty.RegisterReadOnly(nameof(OutputBytes), typeof(Byte[]), typeof(OpenFromBase64Dialog), new PropertyMetadata(default(Byte[])));
        public static readonly DependencyProperty OutputBytesProperty = OutputBytesPropertyKey.DependencyProperty;
        public Byte[] OutputBytes {
            get { return (Byte[])GetValue(OutputBytesProperty); }
            private set { SetValue(OutputBytesPropertyKey, value); }
            }
        #endregion
        #region P:InputBytes:Byte[]
        private static readonly DependencyProperty InputBytesProperty = DependencyProperty.Register(nameof(InputBytes), typeof(Byte[]), typeof(OpenFromBase64Dialog), new PropertyMetadata(default(Byte[]), OnInputBytesChanged));
        private static void OnInputBytesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is OpenFromBase64Dialog source) {
                source.OnInputBytesChanged();
                }
            }

        private void OnInputBytesChanged()
            {
            Text = (InputBytes != null)
                ? Convert.ToBase64String(InputBytes, Base64FormattingOptions.InsertLineBreaks)
                : String.Empty;
            }

        public Byte[] InputBytes {
            get { return (Byte[])GetValue(InputBytesProperty); }
            set { SetValue(InputBytesProperty, value); }
            }
        #endregion
        #region P:ErrorPanelVisibility:Visibility
        private static readonly DependencyPropertyKey ErrorPanelVisibilityPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ErrorPanelVisibility), typeof(Visibility), typeof(OpenFromBase64Dialog), new PropertyMetadata(Visibility.Hidden));
        private static readonly DependencyProperty ErrorPanelVisibilityProperty = ErrorPanelVisibilityPropertyKey.DependencyProperty;
        public Visibility ErrorPanelVisibility {
            get { return (Visibility)GetValue(ErrorPanelVisibilityProperty); }
            private set { SetValue(ErrorPanelVisibilityPropertyKey, value); }
            }
        #endregion
        #region P:ErrorPanelMessage:String
        private static readonly DependencyPropertyKey ErrorPanelMessagePropertyKey = DependencyProperty.RegisterReadOnly(nameof(ErrorPanelMessage), typeof(String), typeof(OpenFromBase64Dialog), new PropertyMetadata(null));
        private static readonly DependencyProperty ErrorPanelMessageProperty = ErrorPanelMessagePropertyKey.DependencyProperty;
        public String ErrorPanelMessage {
            get { return (String)GetValue(ErrorPanelMessageProperty); }
            private set { SetValue(ErrorPanelMessagePropertyKey, value); }
            }
        #endregion

        public static RoutedCommand Yes = new RoutedUICommand();
        public static RoutedCommand No  = new RoutedUICommand();
        public static RoutedCommand FormatCommand  = new RoutedUICommand();

        private void SaveAsCanExecuted(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.Handled = true;
            e.CanExecute = (OutputBytes != null) && (OutputBytes.Length > 0);
            }

        private void FormatCanExecuted(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.Handled = true;
            e.CanExecute = (OutputBytes != null) && (OutputBytes.Length > 0);
            }

        private void YesCanExecuted(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.Handled = true;
            e.CanExecute = (OutputBytes != null) && (OutputBytes.Length > 0);
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
                File.WriteAllBytes(dialog.FileName, OutputBytes);
                }
            }

        private void FormatExecuted(Object sender, ExecutedRoutedEventArgs e)
            {
            e.Handled = true;
            Text = Convert.ToBase64String(OutputBytes, Base64FormattingOptions.InsertLineBreaks);
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

        private void OpenFromBase64Dialog_OnLoaded(Object sender, RoutedEventArgs e)
            {
            Theme.Apply(Theme.Themes[1]);
            }
        }
    }
