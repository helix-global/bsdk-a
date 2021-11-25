using System.Windows;
using System.Windows.Interop;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class WindowsMessageEventArgs : RoutedEventArgs
        {
        public MSG Message { get; }

        public WindowsMessageEventArgs(RoutedEvent evt, MSG message)
          : base(evt)
            {
            Message = message;
            }
        }
    }