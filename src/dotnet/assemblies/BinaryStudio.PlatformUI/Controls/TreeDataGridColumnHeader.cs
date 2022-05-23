using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridColumnHeader : Control
        {
        static TreeDataGridColumnHeader()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridColumnHeader), new FrameworkPropertyMetadata(typeof(TreeDataGridColumnHeader)));
            }
        }
    }