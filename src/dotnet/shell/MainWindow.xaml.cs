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
using BinaryStudio.PlatformUI;
using BinaryStudio.PlatformUI.Shell;
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

        public MainWindow()
            {
            InitializeComponent();
            }

        private void OnLoad(Object sender, RoutedEventArgs e)
            {
            Theme.Apply(Theme.Themes[0]);
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
            LoadFrom(@"C:\TFS\icao\rfid\unsorted\efsod047.p7b");
            }

        private void LoadFrom(String filename) {
            var o = docmanager.LoadView(docmanager.LoadObject(filename));
            docmanager.Add(o, Path.GetFileNameWithoutExtension(filename));
            }

        private void UpdateCommandBindings() {
            CommandManager.RegisterClassCommandBinding(GetType(), new CommandBinding(ApplicationCommands.Open, OpenExecuted,CanExecuteAllways));
            CommandManager.RegisterClassCommandBinding(GetType(), new CommandBinding(DocumentCommands.ConvertToBase64, ConvertToBase64Executed,CanExecuteAllways));;
            }

        private void ConvertToBase64Executed(Object sender, ExecutedRoutedEventArgs e)
            {
            var dialog = new OpenFileDialog();
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
            var dialog = new OpenFileDialog();
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
