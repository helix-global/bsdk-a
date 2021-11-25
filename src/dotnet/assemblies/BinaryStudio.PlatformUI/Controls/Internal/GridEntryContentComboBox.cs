using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    public class GridEntryContentComboBox : ComboBox
        {
        static GridEntryContentComboBox()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridEntryContentComboBox), new FrameworkPropertyMetadata(typeof(GridEntryContentComboBox)));
            }
        }
    }