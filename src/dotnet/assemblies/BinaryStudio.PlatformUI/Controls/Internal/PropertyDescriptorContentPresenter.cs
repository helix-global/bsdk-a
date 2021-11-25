using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    public class PropertyDescriptorContentPresenter : Control
        {
        static PropertyDescriptorContentPresenter()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyDescriptorContentPresenter), new FrameworkPropertyMetadata(typeof(PropertyDescriptorContentPresenter)));
            }
        }
    }