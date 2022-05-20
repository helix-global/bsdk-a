using System.Windows;
using System.Windows.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridRowHeader : DataGridRowHeader
        {
        static TreeDataGridRowHeader()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridRowHeader), new FrameworkPropertyMetadata(typeof(TreeDataGridRowHeader)));
            }
        }
    }