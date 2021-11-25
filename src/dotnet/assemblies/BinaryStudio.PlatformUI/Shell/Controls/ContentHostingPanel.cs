using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class ContentHostingPanel : Control
        {
        static ContentHostingPanel()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentHostingPanel), new FrameworkPropertyMetadata(typeof(ContentHostingPanel)));
            }
        }
    }
