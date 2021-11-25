using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class FloatingElementDockedEventArgs : FloatingOperationFinishedEventArgs
        {
        public DockDirection DockDirection { get; }

        public Boolean CreateDocumentGroup { get; }

        public Int32 InsertPosition { get; }

        public FloatingElementDockedEventArgs(RoutedEvent evt, ViewElement content, DockDirection dockDirection, Boolean createDocumentGroup, Int32 insertPosition)
          : base(evt, content)
            {
            DockDirection = dockDirection;
            CreateDocumentGroup = createDocumentGroup;
            InsertPosition = insertPosition;
            }
        }
    }