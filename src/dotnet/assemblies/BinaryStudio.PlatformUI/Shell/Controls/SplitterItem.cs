using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI
    {
    public class SplitterItem : ContentControl
        {
        static SplitterItem()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterItem), new FrameworkPropertyMetadata(typeof(SplitterItem)));
            }
        }
    }
