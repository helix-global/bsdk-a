using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public abstract class FloatingOperationFinishedEventArgs : RoutedEventArgs
        {
        public ViewElement Content { get; }

        protected FloatingOperationFinishedEventArgs(RoutedEvent evt, ViewElement content)
          : base(evt)
            {
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            Content = content;
            }
        }
    }