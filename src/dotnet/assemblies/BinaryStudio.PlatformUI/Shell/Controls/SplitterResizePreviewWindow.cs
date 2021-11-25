using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI
    {
    public class SplitterResizePreviewWindow : Control
        {
        static SplitterResizePreviewWindow() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterResizePreviewWindow), new FrameworkPropertyMetadata(typeof(SplitterResizePreviewWindow)));
            }

        #region M:Show(UIElement)
        public void Show(UIElement parentElement) {
            var source = PresentationSource.FromVisual(parentElement) as HwndSource;
            var owner = (source == null)
                ? IntPtr.Zero
                : source.Handle;
            EnsureWindow(owner);
            var point = parentElement.PointToScreen(new Point(0.0, 0.0));
            var size = parentElement.RenderSize.LogicalToDeviceUnits();
            HwndSource.SetWindowPosition(point, size,
                WindowSizingAndPositioningFlags.RetainsTheCurrentZOrder |
                WindowSizingAndPositioningFlags.DisplaysTheWindow |
                WindowSizingAndPositioningFlags.DoesNotActivateTheWindow);
            }
        #endregion
        #region M:Hide
        public void Hide() {
            using (HwndSource) { HwndSource = null; }
            }
        #endregion
        #region M:Move(Double,Double)
        public void Move(Double deviceLeft, Double deviceTop) {
            if (HwndSource != null) {
                HwndSource.SetWindowPosition(deviceLeft,deviceTop,0,0,
                    WindowSizingAndPositioningFlags.DisplaysTheWindow |
                    WindowSizingAndPositioningFlags.DoesNotActivateTheWindow |
                    WindowSizingAndPositioningFlags.RetainsTheCurrentSize |
                    WindowSizingAndPositioningFlags.RetainsTheCurrentZOrder);
                }
            }
        #endregion
        #region M:EnsureWindow(IntPtr)
        private void EnsureWindow(IntPtr owner) {
            if (HwndSource == null) {
                const Int32 WindowStyle = unchecked((Int32)((UInt32)(WindowStyles.Disabled | WindowStyles.Topmost | WindowStyles.Transparent | WindowStyles.Popup)));
                var parameters = new HwndSourceParameters(nameof(SplitterResizePreviewWindow)) {
                    Width = 0,
                    Height = 0,
                    PositionX = 0,
                    PositionY = 0,
                    WindowStyle = WindowStyle,
                    UsesPerPixelOpacity = true,
                    ParentWindow = owner
                    };
                HwndSource = new HwndSource(parameters) {
                    SizeToContent = SizeToContent.Manual,
                    RootVisual = this
                    };
                }
            }
        #endregion

        private HwndSource HwndSource;
        }
    }
