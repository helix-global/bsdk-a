using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public struct SetupDockPreviewArgs
        {
        public Rect previewRect;
        public DockTargetType dockTargetType;
        public Point screenPoint;
        public DockDirection dockDirection;
        public FrameworkElement adornedElement;
        public ViewElement floatingElement;
        }
    }