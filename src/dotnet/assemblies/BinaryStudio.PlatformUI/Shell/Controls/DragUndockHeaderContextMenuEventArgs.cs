using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DragUndockHeaderContextMenuEventArgs : RoutedEventArgs
        {
        public Point HeaderPoint { get; }

        public DragUndockHeaderContextMenuEventArgs(RoutedEvent evt, Point headerPoint)
          : base(evt)
            {
            HeaderPoint = headerPoint;
            }
        }
    }