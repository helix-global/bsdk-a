using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class DragUndockHeader : ContentControl, INonClientArea
    {
        public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DragUndockHeader));
        public static readonly RoutedEvent DragAbsoluteEvent = EventManager.RegisterRoutedEvent("DragAbsolute", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DragUndockHeader));
        public static readonly RoutedEvent DragCompletedAbsoluteEvent = EventManager.RegisterRoutedEvent("DragCompletedAbsolute", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteCompletedEventArgs>), typeof(DragUndockHeader));
        public static readonly RoutedEvent DragDeltaEvent = Thumb.DragDeltaEvent.AddOwner(typeof(DragUndockHeader));
        public static readonly RoutedEvent DragHeaderClickedEvent = EventManager.RegisterRoutedEvent("DragHeaderClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragUndockHeader));
        public static readonly RoutedEvent DragHeaderContextMenuEvent = EventManager.RegisterRoutedEvent("DragHeaderContextMenu", RoutingStrategy.Bubble, typeof(EventHandler<DragUndockHeaderContextMenuEventArgs>), typeof(DragUndockHeader));
        private Point originalScreenPoint;
        private Point lastScreenPoint;
        private Boolean movedDuringDrag;
        private HwndSource currentSource;
        private TabItem tabItem;

        private HwndSource CurrentSource {
            get {
                return currentSource;
                }
            set {
                if (currentSource == value)
                    return;
                if (currentSource != null)
                    currentSource.RemoveHook(WndProc);
                currentSource = value;
                if (currentSource == null)
                    return;
                currentSource.AddHook(WndProc);
                }
            }

        #region P:IsDragEnabled:Boolean
        public static readonly DependencyProperty IsDragEnabledProperty = DependencyProperty.Register("IsDragEnabled", typeof(Boolean), typeof(DragUndockHeader), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        public Boolean IsDragEnabled
            {
            get { return (Boolean)GetValue(IsDragEnabledProperty); }
            set { SetValue(IsDragEnabledProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:ViewElement:ViewElement
        public static readonly DependencyProperty ViewElementProperty = DependencyProperty.Register("ViewElement", typeof(ViewElement), typeof(DragUndockHeader));
        public ViewElement ViewElement
            {
            get { return (ViewElement)GetValue(ViewElementProperty); }
            set { SetValue(ViewElementProperty, value); }
            }
        #endregion
        #region P:ViewFrameworkElement:FrameworkElement
        public static readonly DependencyProperty ViewFrameworkElementProperty = DependencyProperty.Register("ViewFrameworkElement", typeof(FrameworkElement), typeof(DragUndockHeader));
        public FrameworkElement ViewFrameworkElement
            {
            get { return (FrameworkElement)GetValue(ViewFrameworkElementProperty); }
            set { SetValue(ViewFrameworkElementProperty, value); }
            }
        #endregion
        #region P:IsWindowTitleBar:Boolean
        public static readonly DependencyProperty IsWindowTitleBarProperty = DependencyProperty.Register("IsWindowTitleBar", typeof(Boolean), typeof(DragUndockHeader), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public Boolean IsWindowTitleBar
            {
            get { return (Boolean)GetValue(IsWindowTitleBarProperty); }
            set { SetValue(IsWindowTitleBarProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:IsDragging:Boolean
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(Boolean), typeof(DragUndockHeader), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
        public Boolean IsDragging
            {
            get { return (Boolean)GetValue(IsDraggingProperty); }
            protected set { SetValue(IsDraggingPropertyKey, Boxes.Box(value)); }
            }
        #endregion

        private Boolean IsInReorderableTabItem {
            get {
                if (tabItem == null)
                    return false;
                return tabItem.FindAncestor<ReorderTabPanel>() != null;
                }
            }

        internal Boolean IsInTabItem {
            get {
                return tabItem != null;
                }
            }

        static DragUndockHeader() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragUndockHeader), new FrameworkPropertyMetadata(typeof(DragUndockHeader)));
            }

        public DragUndockHeader() {
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
            }

        public void CancelDrag() {
            if (!IsDragging)
                return;
            ReleaseCapture();
            RaiseDragCompletedAbsolute(lastScreenPoint, false);
            }

        private void CompleteDrag() {
            if (!IsDragging)
                return;
            ReleaseCapture();
            RaiseDragCompletedAbsolute(lastScreenPoint, movedDuringDrag);
            }

        private void ReleaseCapture() {
            if (!IsDragging)
                return;
            ClearValue(IsDraggingPropertyKey);
            if (!IsMouseCaptured)
                return;
            ReleaseMouseCapture();
            }

        private void BeginDragging(Point screenPoint) {
            if (!CaptureMouse())
                return;
            IsDragging = true;
            originalScreenPoint = screenPoint;
            lastScreenPoint = screenPoint;
            movedDuringDrag = false;
            }

        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);
            var flag = false;
            tabItem = this.FindAncestor<TabItem>();
            if (tabItem != null) {
                var draggedTabInfo = DockManager.Instance.DraggedTabInfo;
                if (draggedTabInfo != null && Equals(draggedTabInfo.DraggedViewElement, ViewElement)) {
                    flag = true;
                    var reorderTabPanel = tabItem.Parent as ReorderTabPanel ?? VisualTreeHelper.GetParent(tabItem) as ReorderTabPanel;
                    if (reorderTabPanel != null && draggedTabInfo != null && (draggedTabInfo.TabStrip != reorderTabPanel || reorderTabPanel.Children.Count != draggedTabInfo.TabRects.Count)) {
                        draggedTabInfo.TabStrip = reorderTabPanel;
                        draggedTabInfo.TabStrip.IsNotificationNeeded = true;
                        }
                    }
                }
            if (!flag || !NativeMethods.IsLeftButtonPressed())
                return;
            BeginDragging(NativeMethods.GetCursorPos());
            }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if (this.IsConnectedToPresentationSource() && IsDragEnabled && !IsWindowTitleBar) {
                BeginDragging(PointToScreen(e.GetPosition(this)));
                RaiseHeaderClicked();
                }
            base.OnMouseLeftButtonDown(e);
            }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
            CaptureMouse();
            base.OnMouseRightButtonDown(e);
            }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (!IsMouseCaptured || !IsDragging || !this.IsConnectedToPresentationSource())
                return;
            movedDuringDrag = true;
            var screen = PointToScreen(e.GetPosition(this));
            RaiseEvent(new DragDeltaEventArgs(screen.X - lastScreenPoint.X, screen.Y - lastScreenPoint.Y));
            RaiseDragAbsolute(screen);
            if (IsOutsideSensitivity(screen)) {
                if (tabItem != null)
                    DockManager.Instance.ComputeTabItemLengths(tabItem);
                RaiseDragStarted(originalScreenPoint);
                }
            lastScreenPoint = screen;
            }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            if (IsMouseCaptured && IsDragging && this.IsConnectedToPresentationSource()) {
                lastScreenPoint = PointToScreen(e.GetPosition(this));
                CompleteDrag();
                }
            base.OnMouseLeftButtonUp(e);
            }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
            if (!IsDragging) {
                if (IsMouseCaptured)
                    ReleaseMouseCapture();
                var source = PresentationSource.FromVisual(this) as HwndSource;
                if (source != null) {
                    if (ShouldShowWindowMenu())
                        CustomChromeWindow.ShowWindowMenu(source, this, e.GetPosition(this), RenderSize);
                    else
                        RaiseEvent(new DragUndockHeaderContextMenuEventArgs(DragHeaderContextMenuEvent, e.GetPosition(this)));
                    }
                e.Handled = true;
                }
            else
                base.OnMouseRightButtonUp(e);
            }

        private Boolean ShouldShowWindowMenu() {
            return IsWindowTitleBar && !IsInTabItem && ViewFrameworkElement == null;
            }

        protected override AutomationPeer OnCreateAutomationPeer() {
            return new DragUndockHeaderAutomationPeer(this);
            }

        private Boolean IsOutsideSensitivity(Point point) {
            var flag = IsInReorderableTabItem;
            var draggedTabInfo = DockManager.Instance.DraggedTabInfo;
            if (draggedTabInfo != null)
                flag = draggedTabInfo.TabStripRect.Contains(point);
            point.Offset(-originalScreenPoint.X, -originalScreenPoint.Y);
            if (flag)
                return false;
            if (Math.Abs(point.X) <= SystemParameters.MinimumHorizontalDragDistance)
                return Math.Abs(point.Y) > SystemParameters.MinimumVerticalDragDistance;
            return true;
            }

        protected void RaiseDragStarted(Point point) {
            RaiseEvent(new DragAbsoluteEventArgs(DragStartedEvent, point));
            }

        internal void RaiseDragAbsolute(Point point) {
            RaiseEvent(new DragAbsoluteEventArgs(DragAbsoluteEvent, point));
            }

        protected void RaiseDragCompletedAbsolute(Point point, Boolean isCompleted) {
            RaiseEvent(new DragAbsoluteCompletedEventArgs(DragCompletedAbsoluteEvent, point, isCompleted));
            }

        protected void RaiseHeaderClicked() {
            RaiseEvent(new RoutedEventArgs(DragHeaderClickedEvent));
            }

        protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e) {
            base.OnIsMouseCapturedChanged(e);
            if (IsMouseCaptured)
                return;
            CancelDrag();
            }

        private void OnSourceChanged(Object sender, SourceChangedEventArgs args) {
            CurrentSource = args.NewSource as HwndSource;
            }

        private IntPtr WndProc(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled) {
            if (msg != 534) {
                if (msg != 561) {
                    if (msg == 562)
                        WmExitSizeMove(ref handled);
                    }
                else
                    WmEnterSizeMove();
                }
            else
                WmMoving(ref handled);
            return IntPtr.Zero;
            }

        internal static Point GetMessagePoint() {
            var messagePos = NativeMethods.GetMessagePos();
            return new Point(NativeMethods.GetXLParam(messagePos), NativeMethods.GetYLParam(messagePos));
            }

        private void WmEnterSizeMove() {
            if (!IsWindowTitleBar)
                return;
            movedDuringDrag = false;
            RaiseDragStarted(GetMessagePoint());
            }

        private void WmExitSizeMove(ref Boolean handled) {
            if (!IsWindowTitleBar)
                return;
            RaiseDragCompletedAbsolute(GetMessagePoint(), movedDuringDrag);
            handled = CurrentSource == null || CurrentSource.IsDisposed;
            }

        private void WmMoving(ref Boolean handled) {
            if (!IsWindowTitleBar)
                return;
            movedDuringDrag = true;
            RaiseDragAbsolute(GetMessagePoint());
            handled = CurrentSource == null || CurrentSource.IsDisposed;
            }

        Int32 INonClientArea.HitTest(Point point) {
            return IsWindowTitleBar ? 2 : 0;
            }
        }
    }
