using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {

    public class WindowResizeGrip : Thumb {
        private SplitterResizePreviewWindow currentPreviewWindow;
        private Rect initialTargetRect;

        #region P:ResizeGripMode:WindowResizeGripMode
        public static readonly DependencyProperty ResizeGripModeProperty = DependencyProperty.Register("ResizeGripMode", typeof(WindowResizeGripMode), typeof(WindowResizeGrip), new PropertyMetadata(WindowResizeGripMode.Splitter));
        public WindowResizeGripMode ResizeGripMode {
            get { return (WindowResizeGripMode)GetValue(ResizeGripModeProperty); }
            set { SetValue(ResizeGripModeProperty, value); }
            }
        #endregion
        #region P:ResizeGripDirection:WindowResizeGripDirection
        public static readonly DependencyProperty ResizeGripDirectionProperty = DependencyProperty.Register("ResizeGripDirection", typeof(WindowResizeGripDirection), typeof(WindowResizeGrip));
        public WindowResizeGripDirection ResizeGripDirection {
            get { return (WindowResizeGripDirection)GetValue(ResizeGripDirectionProperty); }
            set { SetValue(ResizeGripDirectionProperty, value); }
            }
        #endregion
        #region P:ResizeTarget:IResizable
        public static readonly DependencyProperty ResizeTargetProperty = DependencyProperty.Register("ResizeTarget", typeof(IResizable), typeof(WindowResizeGrip));
        public IResizable ResizeTarget {
            get { return (IResizable)GetValue(ResizeTargetProperty); }
            set { SetValue(ResizeTargetProperty, value); }
            }
        #endregion

        private Boolean IsShowingResizePreview {
            get {
                return currentPreviewWindow != null;
                }
            }

        static WindowResizeGrip() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowResizeGrip), new FrameworkPropertyMetadata(typeof(WindowResizeGrip)));
            }

        public WindowResizeGrip() {
            AddHandler(DragStartedEvent, new DragStartedEventHandler(OnDragStarted));
            AddHandler(DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            AddHandler(DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));
            }

        private void OnDragStarted(Object sender, DragStartedEventArgs args) {
            if (ResizeTarget == null)
                return;
            initialTargetRect = ResizeTarget.CurrentBounds;
            if (ResizeGripMode != WindowResizeGripMode.Splitter)
                return;
            currentPreviewWindow = new SplitterResizePreviewWindow();
            currentPreviewWindow.Show(this);
            }

        private void OnDragDelta(Object sender, DragDeltaEventArgs args) {
            if (ResizeTarget == null || PresentationSource.FromVisual(this) == null)
                return;
            switch (ResizeGripMode) {
                case WindowResizeGripMode.Splitter:
                if (!IsShowingResizePreview)
                    break;
                var currentScreenBounds = ResizeTarget.CurrentScreenBounds;
                var deviceUnits1 = ResizeTarget.MinSize.LogicalToDeviceUnits();
                var deviceUnits2 = ResizeTarget.MaxSize.LogicalToDeviceUnits();
                var deviceUnits3 = RenderSize.LogicalToDeviceUnits();
                var screen = PointToScreen(new Point(0.0, 0.0));
                if (ResizeGripDirection.IsResizingHorizontally())
                    screen.X += args.HorizontalChange * DpiHelper.LogicalToDeviceUnitsScalingFactorX;
                if (ResizeGripDirection.IsResizingVertically())
                    screen.Y += args.VerticalChange * DpiHelper.LogicalToDeviceUnitsScalingFactorY;
                if (ResizeGripDirection.IsResizingRight())
                    screen.X = Math.Max(currentScreenBounds.Left + deviceUnits1.Width - deviceUnits3.Width, Math.Min(currentScreenBounds.Left + deviceUnits2.Width - deviceUnits3.Width, screen.X));
                if (ResizeGripDirection.IsResizingLeft())
                    screen.X = Math.Max(currentScreenBounds.Right - deviceUnits2.Width, Math.Min(currentScreenBounds.Right - deviceUnits1.Width, screen.X));
                if (ResizeGripDirection.IsResizingBottom())
                    screen.Y = Math.Max(currentScreenBounds.Top + deviceUnits1.Height - deviceUnits3.Height, Math.Min(currentScreenBounds.Top + deviceUnits2.Height - deviceUnits3.Height, screen.Y));
                if (ResizeGripDirection.IsResizingTop())
                    screen.Y = Math.Max(currentScreenBounds.Bottom - deviceUnits2.Height, Math.Min(currentScreenBounds.Bottom - deviceUnits1.Height, screen.Y));
                currentPreviewWindow.Move(screen.X, screen.Y);
                break;
                case WindowResizeGripMode.DirectUpdate:
                UpdateTargetBounds(args);
                break;
                }
            }

        private void OnDragCompleted(Object sender, DragCompletedEventArgs args) {
            if (ResizeTarget == null)
                return;
            if (ResizeGripMode == WindowResizeGripMode.Splitter) {
                if (!IsShowingResizePreview)
                    return;
                currentPreviewWindow.Hide();
                currentPreviewWindow = null;
                if (args.Canceled)
                    return;
                UpdateTargetBounds(args);
                }
            }

        #region M:UpdateTargetBounds(DragDeltaEventArgs)
        private void UpdateTargetBounds(DragDeltaEventArgs args) {
            UpdateTargetBounds(new Point(args.HorizontalChange, args.VerticalChange));
            }
        #endregion
        #region M:UpdateTargetBounds(DragCompletedEventArgs)
        private void UpdateTargetBounds(DragCompletedEventArgs args) {
            UpdateTargetBounds(new Point(args.HorizontalChange, args.VerticalChange).DeviceToLogicalUnits());
            }
        #endregion
        #region M:UpdateTargetBounds(Point)
        private void UpdateTargetBounds(Point pt) {
            if ((pt.X == 0.0) && (pt.Y == 0.0)) { return; }
            switch (ResizeGripDirection) {
                case WindowResizeGripDirection.Left:        { ResizeTarget.UpdateBounds(pt.X,  0.0, -pt.X,   0.0); } break;
                case WindowResizeGripDirection.Right:       { ResizeTarget.UpdateBounds( 0.0,  0.0,  pt.X,   0.0); } break;
                case WindowResizeGripDirection.Top:         { ResizeTarget.UpdateBounds( 0.0, pt.Y,   0.0, -pt.Y); } break;
                case WindowResizeGripDirection.Bottom:      { ResizeTarget.UpdateBounds( 0.0,  0.0,   0.0,  pt.Y); } break;
                case WindowResizeGripDirection.TopLeft:     { ResizeTarget.UpdateBounds(pt.X, pt.Y, -pt.X, -pt.Y); } break;
                case WindowResizeGripDirection.TopRight:    { ResizeTarget.UpdateBounds( 0.0, pt.Y,  pt.X, -pt.Y); } break;
                case WindowResizeGripDirection.BottomLeft:  { ResizeTarget.UpdateBounds(pt.X,  0.0, -pt.X,  pt.Y); } break;
                case WindowResizeGripDirection.BottomRight: { ResizeTarget.UpdateBounds( 0.0,  0.0,  pt.X,  pt.Y); } break;
                }
            }
        #endregion
        }
    }
