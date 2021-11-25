using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DragAbsoluteCompletedEventArgs : DragAbsoluteEventArgs
        {
        public Boolean IsCompleted { get; set; }

        public DragAbsoluteCompletedEventArgs(RoutedEvent source, Point point, Boolean isCompleted)
          : base(source, point)
            {
            IsCompleted = isCompleted;
            }
        }
    }