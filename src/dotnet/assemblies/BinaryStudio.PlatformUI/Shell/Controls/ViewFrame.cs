using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class ViewFrame : ViewPresenter
        {
        static ViewFrame() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewFrame), new FrameworkPropertyMetadata(typeof(ViewFrame)));
            }
        }
    }
