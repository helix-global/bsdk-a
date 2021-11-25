using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Interop;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockAdornerWindow : ContentControl
        {
        private IntPtr ownerHwnd;
        private DockTarget dockTarget;
        private HwndSource window;
        private DockAdorner innerContent;
        private const Double OutAdornmentOffset = 10.0;

        public Boolean IsDockGroup
            {
            get
                {
                return DockDirection == DockDirection.Fill;
                }
            }

        public FrameworkElement AdornedElement { get; set; }
        public DockDirection DockDirection { get; set; }
        public Orientation? Orientation { get; set; }
        public Boolean AreOuterTargetsEnabled { get; set; }
        public Boolean AreInnerTargetsEnabled { get; set; }
        public Boolean IsInnerCenterTargetEnabled { get; set; }
        public Boolean AreInnerSideTargetsEnabled { get; set; }

        public DockAdornerWindow(IntPtr ownerHwnd)
            {
            this.ownerHwnd = ownerHwnd;
            }

        public void PrepareAndShow()
            {
            var adornedElement = AdornedElement as DockTarget;
            if (dockTarget != adornedElement)
                {
                PrepareAndHide();
                dockTarget = adornedElement;
                }
            if (window == null)
                {
                UpdateContent();
                window = new HwndSource(new HwndSourceParameters
                    {
                    Width = 0,
                    Height = 0,
                    ParentWindow = ownerHwnd,
                    UsesPerPixelOpacity = true,
                    WindowName = "DockAdornerWindow",
                    WindowStyle = -2013265880
                    });
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.RootVisual = this;
                DockManager.Instance.RegisterSite(this, window.Handle);
                }
            UpdatePositionAndVisibility();
            }

        public void PrepareAndHide()
            {
            if (window == null)
                return;
            DockManager.Instance.UnregisterSite(this);
            Content = innerContent = null;
            window.Dispose();
            window = null;
            }

        private void UpdatePositionAndVisibility()
            {
            if (!IsArrangeValid)
                UpdateLayout();
            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;
            var num1 = actualWidth - AdornedElement.ActualWidth;
            var num2 = actualHeight - AdornedElement.ActualHeight;
            var logicalUnits = DpiHelper.DeviceToLogicalUnits(AdornedElement.PointToScreen(new Point(0.0, 0.0)));
            RECT lpRect;
            NativeMethods.GetWindowRect(ownerHwnd, out lpRect);
            var point2 = new Point(lpRect.Left, lpRect.Top);
            var vector = Point.Subtract(logicalUnits, point2);
            var num3 = vector.X - num1 / 2.0;
            var num4 = vector.Y - num2 / 2.0;
            var num5 = vector.X + 10.0;
            var num6 = vector.X - actualWidth + AdornedElement.ActualWidth - 10.0;
            var num7 = vector.Y + 10.0;
            var num8 = vector.Y - actualHeight + AdornedElement.ActualHeight - 10.0;
            var offsetX = 0.0;
            var offsetY = 0.0;
            switch (DockDirection)
                {
                case DockDirection.Fill:
                    offsetX = num3;
                    offsetY = num4;
                    break;
                case DockDirection.Left:
                    offsetX = num5;
                    offsetY = num4;
                    break;
                case DockDirection.Top:
                    offsetX = num3;
                    offsetY = num7;
                    break;
                case DockDirection.Right:
                    offsetX = num6;
                    offsetY = num4;
                    break;
                case DockDirection.Bottom:
                    offsetX = num3;
                    offsetY = num8;
                    break;
                }
            point2.Offset(offsetX, offsetY);
            var deviceUnits = DpiHelper.LogicalToDeviceUnits(point2);
            NativeMethods.SetWindowPos(window.Handle, IntPtr.Zero, (Int32)deviceUnits.X, (Int32)deviceUnits.Y, 0, 0, 85);
            }

        private void UpdateContent()
            {
            var dockAdorner = !IsDockGroup ? new DockSiteAdorner() : (DockAdorner)new DockGroupAdorner();
            dockAdorner.AdornedElement = AdornedElement;
            dockAdorner.DockDirection = DockDirection;
            dockAdorner.Orientation = Orientation;
            dockAdorner.AreOuterTargetsEnabled = AreOuterTargetsEnabled;
            dockAdorner.AreInnerTargetsEnabled = AreInnerTargetsEnabled;
            dockAdorner.IsInnerCenterTargetEnabled = IsInnerCenterTargetEnabled;
            dockAdorner.AreInnerSideTargetsEnabled = AreInnerSideTargetsEnabled;
            Content = innerContent = dockAdorner;
            innerContent.UpdateContent();
            }

        /// <summary>Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementations for the Windows Presentation Foundation (WPF) infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new DockAdornerWindowAutomationPeer(this);
            }
        }
    }