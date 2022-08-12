using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace PlatformUISample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

    private void MainWindow_OnLoaded(Object sender, RoutedEventArgs e)
        {
        Theme.Apply();
        foreach (var theme in Theme.Themes) {
            MenuItem i;
            ThemeMenuItem.Items.Add(i = new MenuItem
                {
                Header = theme,
                });
            i.Click += ThemeChange;
            }

        Colors.ItemsSource = typeof(SystemColors).
                GetProperties(BindingFlags.Public|BindingFlags.Static).
                Where(i => i.PropertyType == typeof(SolidColorBrush)).
                Select(i => Tuple.Create(i.GetValue(null, null),i.Name)).
                ToArray();
        }

    private void MenuItem_OnClick(Object sender, RoutedEventArgs e)
        {
        throw new NotImplementedException();
        }

    private void ThemeChange(Object sender, RoutedEventArgs e)
        {
        Theme.Apply((Theme)((MenuItem)e.OriginalSource).Header);
        }
    }
}
