using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class ViewEventArgs : RoutedEventArgs
        {
        public View View { get; }

        public ViewEventArgs(RoutedEvent evt, View view)
          : base(evt)
            {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            View = view;
            }
        }
    }