using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BinaryStudio.IO;
using BinaryStudio.PlatformUI;
using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using Microsoft.Win32;
using Path=System.IO.Path;

namespace shell
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BinaryStudio.PlatformUI.MainWindow
        {
        private DocumentGroup dockgroup;
        private DocumentManager docmanager;

        static MainWindow()
            {
            EventManager.RegisterClassHandler(typeof(MainWindow), CommandSource.OpenBinaryDataEvent, new RoutedEventHandler(OpenBinaryDataExecuted));
            }

        private static void OpenBinaryDataExecuted(Object sender, RoutedEventArgs e) {
            if (sender is MainWindow source) {
                source.OpenBinaryDataExecuted(e);
                }
            }

        public MainWindow()
            {
            InitializeComponent();
            }

        private void OnLoad(Object sender, RoutedEventArgs e)
            {
            Theme.Apply(Theme.Themes[7]);
            UpdateCommandBindings();
            var dockgroupcontainer = (DocumentGroupContainer)Profile.DockRoot.Children.FirstOrDefault(i => i is DocumentGroupContainer);
            if (dockgroupcontainer == null) {
                Profile.DockRoot.Children.Add(new DocumentGroupContainer
                    (
                    dockgroup = new DocumentGroup())
                    );
                }
            else
                {
                dockgroup = (DocumentGroup)dockgroupcontainer.Children.FirstOrDefault(i => i is DocumentGroup);
                if (dockgroup == null)
                    {
                    dockgroupcontainer.Children.Add(dockgroup = new DocumentGroup());
                    }
                }
            ViewManager.GetViewManager(dockgroup);
            docmanager = new DocumentManager(dockgroup);
            Initialize();
            }

        private void Initialize()
            {
            //LoadFrom(@"C:\TFS\bsdk\src\dotnet\samples\SmartCardSample\bin\Debug\net40\.sqlite3\trace-SmartCardSample-2022-05-17-18-03-17.db");
            //docmanager.AddCertificateStoreManagement();
            }

        private void LoadFrom(String filename) {
            var o = docmanager.LoadView(docmanager.LoadObject(filename));
            docmanager.Add(o, Path.GetFileNameWithoutExtension(filename));
            }

        private void UpdateCommandBindings() {
            CommandManager.RegisterClassCommandBinding(GetType(), new CommandBinding(ApplicationCommands.Open, OpenExecuted,CanExecuteAllways));
            CommandManager.RegisterClassCommandBinding(GetType(), new CommandBinding(DocumentCommands.ConvertToBase64, ConvertToBase64Executed,CanExecuteAllways));
            CommandManager.RegisterClassCommandBinding(GetType(), new CommandBinding(DocumentCommands.OpenBase64, OpenBase64Executed,CanExecuteAllways));;
            }

        private void OpenBase64Executed(Object sender, ExecutedRoutedEventArgs e)
            {
            var dialog = new OpenFromBase64Dialog
                {
                Owner = this
                };
            Theme.CurrentTheme.ApplyTo(dialog);
            if (dialog.ShowDialog() == true)
                {
                var o = docmanager.LoadView(Asn1Object.Load(new ReadOnlyMemoryMappingStream(dialog.OutputBytes)).FirstOrDefault());
                docmanager.Add(o, "?");
                }
            }

        private void OpenBinaryDataExecuted(RoutedEventArgs e) {
            if (e is OpenBinaryDataEventArgs data) {
                var dialog = new OpenFromBase64Dialog{
                    Owner = this,
                    InputBytes = data.Data
                    };
                if (dialog.ShowDialog() == true)
                    {
                    var o = docmanager.LoadView(Asn1Object.Load(new ReadOnlyMemoryMappingStream(dialog.OutputBytes)).FirstOrDefault());
                    docmanager.Add(o, "?");
                    e.Handled = true;
                    }
                }
            }

        private void ConvertToBase64Executed(Object sender, ExecutedRoutedEventArgs e)
            {
            var dialog = new OpenFileDialog{
                Filter = "All Files (*.*)|*.*"
                };
            if (dialog.ShowDialog(this) == true)
                {
                var r = File.ReadAllBytes(dialog.FileName);
                docmanager.Add(
                    new[] {
                        new View<EBase64Edit>(new EBase64Edit {
                        Text = Convert.ToBase64String(r)
                        }) },
                    Path.GetFileNameWithoutExtension(dialog.FileName));
                }
            }

        private void OpenExecuted(Object sender, ExecutedRoutedEventArgs e)
            {
            var dialog = new OpenFileDialog{
                Filter = "All Files (*.*)|*.*|SQLite Files (*.db)|*.db"
                };
            if (dialog.ShowDialog(this) == true)
                {
                LoadFrom(dialog.FileName);
                }
            }

        private static void CanExecuteAllways(Object sender, CanExecuteRoutedEventArgs e)
            {
            e.CanExecute = true;
            e.Handled = true;
            }
        }
    }
