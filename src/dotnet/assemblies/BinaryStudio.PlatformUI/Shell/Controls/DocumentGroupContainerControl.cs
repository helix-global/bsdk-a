using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DocumentGroupContainerControl : DockGroupControl
        {
        static DocumentGroupContainerControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentGroupContainerControl), new FrameworkPropertyMetadata(typeof(DocumentGroupContainerControl)));
            }
        }
    }
