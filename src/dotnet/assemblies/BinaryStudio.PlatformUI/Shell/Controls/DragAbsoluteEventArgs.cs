using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class DragAbsoluteEventArgs : RoutedEventArgs {
        public Point ScreenPoint { get; }
        public DragAbsoluteEventArgs(RoutedEvent source, Point point)
            :base(source)
            {
            ScreenPoint = point;
            }
        }
    }