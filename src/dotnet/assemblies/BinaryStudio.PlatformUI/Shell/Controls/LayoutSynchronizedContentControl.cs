using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI
    {
    public class LayoutSynchronizedContentControl : ContentControl
        {
        static LayoutSynchronizedContentControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutSynchronizedContentControl), new FrameworkPropertyMetadata(typeof(LayoutSynchronizedContentControl)));
            }
        }
    }
