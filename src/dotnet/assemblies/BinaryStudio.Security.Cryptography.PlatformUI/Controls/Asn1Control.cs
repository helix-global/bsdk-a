using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Controls
{
    public class Asn1Control : Control, INotifyPropertyChanged
        {
        static Asn1Control()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Asn1Control), new FrameworkPropertyMetadata(typeof(Asn1Control)));
            DataContextProperty.OverrideMetadata(typeof(Asn1Control),  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnDataContextChanged));
            }

        public static RoutedCommand CopyBase64 = new RoutedUICommand();
        public static RoutedCommand CopyJson   = new RoutedUICommand();
        public static RoutedCommand CopyCrtJson = new RoutedUICommand();
        public static RoutedCommand CopyCmsJson = new RoutedUICommand();
        public static RoutedCommand CopyCrlJson = new RoutedUICommand();

        public Asn1GridEntryColumnDefinition[] Columns { get; }

        public Asn1Control()
            {
            Columns = new []
                {
                new Asn1GridEntryColumnDefinition{ Width = new GridLength(200) },
                new Asn1GridEntryColumnDefinition{ Width = new GridLength(50) },
                new Asn1GridEntryColumnDefinition{ Width = new GridLength(50) },
                new Asn1GridEntryColumnDefinition{ Width = new GridLength(50) },
                new Asn1GridEntryColumnDefinition{ Width = new GridLength(50) }
                };
            CommandManager.AddCanExecuteHandler(this, OnCanExecuteCommand);
            CommandManager.AddExecutedHandler(this, OnExecutedCommand);
            }

        #region P:DataContext:Object
        private static void OnDataContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = sender as Asn1Control;
            if (source != null) {
                source.OnDataContextChanged();
                }
            }

        private void OnDataContextChanged() {
            var r = DataContext as Asn1Object;
            if (r == null) {
                var i = DataContext as IServiceProvider;
                if (i != null) {
                    r = i.GetService(typeof(Asn1Object)) as Asn1Object;
                    }
                }
            if (ItemsHost != null) {
                ItemsHost.ItemsSource = (r != null)
                    ? new []{ new Asn1GridEntry(this, (r is Asn1LinkObject) ? ((Asn1LinkObject)r).UnderlyingObject : r) }
                    : null;
                }
            }
        #endregion
        #region P:DecodeObjectIdentifier:Boolean
        public static readonly DependencyProperty DecodeObjectIdentifierProperty = DependencyProperty.Register("DecodeObjectIdentifier", typeof(Boolean), typeof(Asn1Control), new PropertyMetadata(default(Boolean), OnDecodeObjectIdentifierChanged));
        private static void OnDecodeObjectIdentifierChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as Asn1Control);
            if (source != null)
                {
                source.OnDecodeObjectIdentifierChanged();
                }
            }

        private void OnDecodeObjectIdentifierChanged()
            {
            OnPropertyChanged(nameof(DecodeObjectIdentifier));
            }

        public Boolean DecodeObjectIdentifier {
            get { return (Boolean)GetValue(DecodeObjectIdentifierProperty); }
            set { SetValue(DecodeObjectIdentifierProperty, value); }
            }
        #endregion

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            if (ItemsHost == null) {
                ItemsHost = GetTemplateChild("ItemsHost") as TreeView;
                if (ItemsHost != null) {
                    OnDataContextChanged();
                    }
                }
            HostGrid = GetTemplateChild("HostGrid") as Grid;
            if (HostGrid != null) {
                Columns[0].Definition = HostGrid.ColumnDefinitions[0];
                Columns[1].Definition = HostGrid.ColumnDefinitions[1];
                Columns[2].Definition = HostGrid.ColumnDefinitions[2];
                Columns[3].Definition = HostGrid.ColumnDefinitions[3];
                Columns[4].Definition = HostGrid.ColumnDefinitions[4];
                }
            }

        private static Asn1Certificate ReadCrt(Asn1Object o) {
            if (o != null) {
                try
                    {
                    var r = new Asn1Certificate(o);
                    if (!r.IsFailed) { return r; }
                    }
                catch
                    {
                    return null;
                    }
                }
            return null;
            }

        private static Asn1CertificateRevocationList ReadCrl(Asn1Object o) {
            if (o != null) {
                try
                    {
                    var r = new Asn1CertificateRevocationList(o);
                    if (!r.IsFailed) { return r; }
                    }
                catch
                    {
                    return null;
                    }
                }
            return null;
            }

        private static CmsMessage ReadCms(Asn1Object o) {
            if (o != null) {
                try
                    {
                    var r = new CmsMessage(o);
                    if (!r.IsFailed) { return r; }
                    }
                catch
                    {
                    return null;
                    }
                }
            return null;
            }

        private static MenuItem CreateMenuItem(Asn1Object o, String header, RoutedCommand command) {
            if (o != null) {
                var r = new MenuItem {
                    Header = header,
                    Command = command,
                    CommandParameter = o
                    };
                return r;
                }
            return null;
            }

        #region M:OnPreviewMouseRightButtonDown(MouseButtonEventArgs)
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseRightButtonDown" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the right mouse button was pressed.</param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e) {
            e.Handled = true;
            if (e.OriginalSource is FrameworkElement i) {
                if (i.DataContext is Asn1GridEntry context) {
                    var contextmenu = new ContextMenu();
                    contextmenu.Items.Add(new MenuItem{
                        Header = "Save...",
                        Command = ApplicationCommands.Save,
                        CommandParameter = context
                        });
                    contextmenu.Items.Add(new MenuItem{
                        Header = "Copy (BASE64)",
                        Command = CopyBase64,
                        CommandParameter = context
                        });
                    contextmenu.Items.Add(new MenuItem{
                        Header = "Copy (Json)",
                        Command = CopyJson,
                        CommandParameter = context
                        });
                    var menuitem =
                        CreateMenuItem(ReadCrt(context.source), "Copy (Json-Certificate)", CopyCrtJson) ??
                        CreateMenuItem(ReadCrl(context.source), "Copy (Json-CRL)", CopyCrlJson) ??
                        CreateMenuItem(ReadCms(context.source), "Copy (Json-CMS)", CopyCmsJson);
                    if (menuitem != null) {
                        contextmenu.Items.Add(menuitem);
                        }
                    foreach (var command in optionalcommands) {
                        var cc = command.Command;
                        var rc = command.Command as RoutedUICommand;
                        if ((rc != null)
                            ? rc.CanExecute(context.source, command.CommandTarget)
                            : cc.CanExecute(context.source)) {
                            contextmenu.Items.Add(new MenuItem{
                                Header = (rc != null) ? rc.Text : command.ToString(),
                                Command = command.Command,
                                CommandParameter = context.source
                                });
                            }
                        }
                    var pt = e.GetPosition(this);
                    contextmenu.PlacementTarget = this;
                    contextmenu.Placement = PlacementMode.MousePoint;
                    contextmenu.PlacementRectangle = new Rect(pt.X,pt.Y,0,0);
                    contextmenu.IsOpen = true;
                    }
                }
            }
        #endregion
        #region M:OnPropertyChanged(String)
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        #endregion
        #region M:OnPropertyChanged(DependencyPropertyChangedEventArgs)
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            OnPropertyChanged(e.Property.Name);
            }
        #endregion
        #region M:OnCanExecuteCommand(Object,CanExecuteRoutedEventArgs)
        protected internal void OnCanExecuteCommand(Object sender, CanExecuteRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, ApplicationCommands.SaveAs)) {
                    var asn1 = DataContext as Asn1Object;
                    e.Handled = true;
                    e.CanExecute = (asn1 != null) && (asn1.Count > 0);
                    }
                else if (ReferenceEquals(e.Command, CopyJson) ||
                         ReferenceEquals(e.Command, CopyBase64)) {
                    e.Handled = true;
                    e.CanExecute = e.Parameter is Asn1GridEntry;
                    }
                else if (ReferenceEquals(e.Command, ApplicationCommands.Save)) {
                    e.Handled = true;
                    e.CanExecute = e.Parameter is Asn1GridEntry;
                    }
                else if (ReferenceEquals(e.Command, NavigationCommands.Zoom)) {
                    e.Handled = true;
                    e.CanExecute = true;
                    }
                else
                    {
                    e.Handled = true;
                    e.CanExecute = true;
                    }
                }
            }
        #endregion
        #region M:OnExecutedCommand(ExecutedRoutedEventArgs)
        protected internal void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, ApplicationCommands.SaveAs)) {
                    e.Handled = true;
                         if (e.Parameter is Asn1GridEntry entry) { SaveAs(entry); }
                    else if (e.Parameter is Asn1Object o)        { SaveAs(o);     }
                    }
                else if (ReferenceEquals(e.Command, CopyBase64)) {
                    e.Handled = true;
                    if (e.Parameter is Asn1GridEntry o) {
                        Copy(o, "BASE64");
                        }
                    }
                else if (
                    ReferenceEquals(e.Command, CopyJson) ||
                    ReferenceEquals(e.Command, CopyCrtJson) ||
                    ReferenceEquals(e.Command, CopyCrlJson) ||
                    ReferenceEquals(e.Command, CopyCmsJson)) {
                    e.Handled = true;
                    if (e.Parameter is Asn1GridEntry o) {
                        Copy(o, "JSON");
                        }
                    else if (e.Parameter is Asn1Object o2) {
                        Copy(o2, "JSON");
                        }
                    }
                else if (ReferenceEquals(e.Command, ApplicationCommands.Save)) {
                    e.Handled = true;
                    SaveAs(e.Parameter as Asn1GridEntry);
                    }
                //else if (ReferenceEquals(e.Command, NavigationCommands.Zoom)) {
                //    e.Handled = true;
                //    var builder = new DefaultBuilder(HostGrid.ActualWidth, HostGrid.ActualHeight);
                //    builder.Metafile.Render(HostGrid, RenderType.Draft);
                //    //builder.DrawRectangle(new Pen(Brushes.Black, 1), new Rect(0,0,30,30));
                //    var window = this.FindAll<Window>(true).FirstOrDefault();
                //    builder.CopyToClipboard(window);
                //    }
                }
            }
        #endregion
        #region M:SaveAs(Asn1GridEntry)
        private void SaveAs(Asn1GridEntry e) {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }
            if (e.source == null) { throw new ArgumentOutOfRangeException(nameof(e)); }
            SaveAs(e.source);
            }
        #endregion
        #region M:SaveAs(Asn1Object)
        private void SaveAs(Asn1Object o) {
            if (o == null) { throw new ArgumentNullException(nameof(o)); }
            var filter = new List<String>();
            if (ReadCrt(o) != null) { filter.Add("Certificate Files (*.cer)|*.cer"); }
            if (ReadCrl(o) != null) { filter.Add("Certificate Revocation List Files (*.crl)|*.crl"); }
            if (ReadCms(o) != null) { filter.Add("PKCS #7 Files (*.p7b)|*.p7b"); }
            filter.Add("Basic Encoding Rules Files (*.ber)|*.ber");
            filter.Add("All Files|*.*");
            var dialog = new SaveFileDialog {
                Filter = String.Join("|", filter)
                };
            if (dialog.ShowDialog(null) == true) {
                using (var output = File.Create(dialog.FileName)) {
                    o.Write(output);
                    }
                }
            }
        #endregion
        #region M:Copy(Asn1GridEntry)
        private void Copy(Asn1GridEntry o, String type) {
            if (o == null) { throw new ArgumentNullException(nameof(o)); }
            Copy(o.source, type);
            }
        #endregion
        #region M:Copy(Asn1Object,TextDataFormat)
        private void Copy(Asn1Object o, String type) {
            if (o == null) { throw new ArgumentNullException(nameof(o)); }
            if (type == "BASE64") {
                using (var output = new MemoryStream()) {
                    o.Write(output);
                    output.Seek(0, SeekOrigin.Begin);
                    Clipboard.SetText(Convert.ToBase64String(output.ToArray()), TextDataFormat.UnicodeText);
                    }
                }
            else if (type == "JSON") {
                var builder = new StringBuilder();
                using (var writer = new JsonTextWriter(new StringWriter(builder)){
                        Formatting = Formatting.Indented,
                        Indentation = 2,
                        IndentChar = ' '
                        })
                    {
                    var serializer = new JsonSerializer();
                    o.WriteJson(writer, serializer);
                    writer.Flush();
                    }
                Clipboard.SetText(builder.ToString(), TextDataFormat.UnicodeText);
                }
            }
        #endregion

        private TreeView ItemsHost { get; set; }
        private Grid HostGrid { get; set; }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
            {
            base.OnMouseWheel(e);
            e.Handled = false;
            }

        private static readonly ISet<ICommandSource> optionalcommands = new HashSet<ICommandSource>();
        public static void RegisterOptionalCommand(ICommandSource command) {
            if (command == null) { throw new ArgumentNullException(nameof(command)); }
            optionalcommands.Add(command);
            }
        }
    }
