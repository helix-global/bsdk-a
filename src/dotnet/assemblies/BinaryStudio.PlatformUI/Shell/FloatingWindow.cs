using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class FloatingWindow : FloatingElement
        {
        public static readonly RoutedEvent AppCommandMessageReceived = EventManager.RegisterRoutedEvent("WindowsMessageReceived", RoutingStrategy.Direct, typeof(EventHandler<WindowsMessageEventArgs>), typeof(FloatingWindow));
        public static readonly RoutedEvent IsFloatingWindowDragWithinChangedEvent = EventManager.RegisterRoutedEvent("IsFloatingWindowDragWithinChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FloatingWindow));
        public static readonly DependencyProperty IsFloatingWindowDragWithinProperty = DependencyProperty.RegisterAttached("IsFloatingWindowDragWithin", typeof(Boolean), typeof(FloatingWindow), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsFloatingWindowDragWithinChanged));
        public static readonly RoutedEvent LocationChangedEvent = EventManager.RegisterRoutedEvent("LocationChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FloatingWindow));
        private Boolean IsUpdatingWindowPlacement;

        #region P:ViewSite:ViewSite
        public static readonly DependencyProperty ViewSiteProperty = DependencyProperty.Register("ViewSite", typeof(ViewSite), typeof(FloatingWindow), new PropertyMetadata(default(ViewSite), OnViewSiteChanged));
        public ViewSite ViewSite {
            get { return (ViewSite)GetValue(ViewSiteProperty); }
            set { SetValue(ViewSiteProperty, value); }
            }

        #region M:OnViewSiteChanged(DependencyObject,DependencyPropertyChangedEventArgs)
        private static void OnViewSiteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (FloatingWindow)sender;
            source.DisconnectViewSiteEvents(e.OldValue as ViewSite);
            source.ConnectViewSiteEvents(e.NewValue as ViewSite);
            }
        #endregion
        #endregion
        #region P:FloatingWindow.CloseAllViews:Boolean
        public static readonly DependencyProperty CloseAllViewsProperty = DependencyProperty.RegisterAttached("CloseAllViews", typeof(Boolean), typeof(FloatingWindow), new PropertyMetadata(default(Boolean)));
        public static void SetCloseAllViews(DependencyObject source, Boolean value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(CloseAllViewsProperty, value);
            }
        public static Boolean GetCloseAllViews(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Boolean)source.GetValue(CloseAllViewsProperty);
            }
        #endregion
        #region P:FloatingWindow.IsFloating:Boolean
        public static readonly DependencyProperty IsFloatingProperty = DependencyProperty.RegisterAttached("IsFloating", typeof(Boolean), typeof(FloatingWindow), new FrameworkPropertyMetadata(default(Boolean), FrameworkPropertyMetadataOptions.Inherits));
        public static void SetIsFloating(DependencyObject source, Boolean value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(IsFloatingProperty, value);
            }
        public static Boolean GetIsFloating(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Boolean)source.GetValue(IsFloatingProperty);
            }
        #endregion
        #region P:FloatingWindow.IsUndockingTab:Boolean
        private static readonly DependencyPropertyKey IsUndockingTabPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsUndockingTab", typeof(Boolean), typeof(FloatingWindow), new FrameworkPropertyMetadata(default(Boolean), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, OnIsUndockingTabChanged));
        public static readonly DependencyProperty IsUndockingTabProperty = IsUndockingTabPropertyKey.DependencyProperty;
        private static void SetIsUndockingTab(DependencyObject source, Boolean value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(IsUndockingTabPropertyKey, value);
            }
        public static Boolean GetIsUndockingTab(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Boolean)source.GetValue(IsUndockingTabProperty);
            }
        #endregion
        #region P:FloatingWindow.HasAutohiddenViews:Boolean
        public static readonly DependencyProperty HasAutohiddenViewsProperty = DependencyProperty.RegisterAttached("HasAutohiddenViews", typeof(Boolean), typeof(FloatingWindow), new FrameworkPropertyMetadata(default(Boolean), FrameworkPropertyMetadataOptions.Inherits));
        public static void SetHasAutohiddenViews(DependencyObject source, Boolean value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(HasAutohiddenViewsProperty, value);
            }
        public static Boolean GetHasAutohiddenViews(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Boolean)source.GetValue(HasAutohiddenViewsProperty);
            }
        #endregion
        #region P:FloatingWindow.HasDocumentGroupContainer:Boolean
        public static readonly DependencyProperty HasDocumentGroupContainerProperty = DependencyProperty.RegisterAttached("HasDocumentGroupContainer", typeof(Boolean), typeof(FloatingWindow), new FrameworkPropertyMetadata(default(Boolean), FrameworkPropertyMetadataOptions.Inherits));
        public static void SetHasDocumentGroupContainer(DependencyObject source, Boolean value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(HasDocumentGroupContainerProperty, value);
            }
        public static Boolean GetHasDocumentGroupContainer(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Boolean)source.GetValue(HasDocumentGroupContainerProperty);
            }
        #endregion
        #region P:FloatingWindow.OnScreenViewCardinality:OnScreenViewCardinality
        public static readonly DependencyProperty OnScreenViewCardinalityProperty = DependencyProperty.RegisterAttached("OnScreenViewCardinality", typeof(OnScreenViewCardinality), typeof(FloatingWindow), new FrameworkPropertyMetadata(OnScreenViewCardinality.One, FrameworkPropertyMetadataOptions.Inherits));
        public static void SetOnScreenViewCardinality(DependencyObject source, OnScreenViewCardinality value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(OnScreenViewCardinalityProperty, value);
            }
        public static OnScreenViewCardinality GetOnScreenViewCardinality(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (OnScreenViewCardinality)source.GetValue(OnScreenViewCardinalityProperty);
            }
        #endregion
        #region P:ShouldShowGlow:Boolean
        protected override Boolean ShouldShowGlow { get {
            return base.ShouldShowGlow && !GetIsUndockingTab(this);
            }}
        #endregion

        static FloatingWindow() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(typeof(FloatingWindow)));
            LeftProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(OnLeftOrTopChanged));
            TopProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(OnLeftOrTopChanged));
            }

        public FloatingWindow() {
            SetIsFloating(this, true);
            SetBinding(WindowStateProperty, new Binding {
                Path = new PropertyPath("FloatingWindowState"),
                Mode = BindingMode.TwoWay
                });
            }

        #region M:ConnectViewSiteEvents(ViewSite)
        private void ConnectViewSiteEvents(ViewSite site) {
            if (site != null) {
                site.FloatingPositionChanged += OnFloatingPositionChanged;
                site.FloatingSizeChanged += OnFloatingSizeChanged;
                }
            }
        #endregion
        #region M:DisconnectViewSiteEvents(ViewSite)
        private void DisconnectViewSiteEvents(ViewSite site) {
            if (site != null) {
                site.FloatingPositionChanged -= OnFloatingPositionChanged;
                site.FloatingSizeChanged -= OnFloatingSizeChanged;
                }
            }
        #endregion
        #region M:OnFloatingPositionChanged(Object,EventArgs)
        private void OnFloatingPositionChanged(Object sender, EventArgs e) {
            var source = (ViewElement)sender;
            if (IsUpdatingWindowPlacement) { return; }
            var pt = Screen.RelativePositionToAbsolutePosition(source.Display, source.FloatingLeft, source.FloatingTop);
            SetCurrentValue(LeftProperty, pt.X);
            SetCurrentValue(TopProperty, pt.Y);
            }
        #endregion
        #region M:OnFloatingSizeChanged(Object,EventArgs)
        private void OnFloatingSizeChanged(Object sender, EventArgs e) {
            var source = (ViewElement)sender;
            if (IsUpdatingWindowPlacement) { return; }
            SetCurrentValue(WidthProperty, source.FloatingWidth);
            SetCurrentValue(HeightProperty, source.FloatingHeight);
            }
        #endregion
        #region M:OnClosed(EventArgs)
        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            DisconnectViewSiteEvents(ViewSite);
            }
        #endregion
        #region M:OnLayoutUpdated(Object,EventArgs)
        private void OnLayoutUpdated(Object sender, EventArgs e) {
            if (GetIsUndockingTab(this))
                UpdateClipRegion(ClipRegionChangeType.FromUndockSingleTab);
            else
                LayoutUpdated -= OnLayoutUpdated;
            }
        #endregion
        #region M:OnContentRendered(EventArgs)
        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            if (!GetIsUndockingTab(this) || NativeMethods.IsLeftButtonPressed()) { return; }
            SetIsUndockingTab(this, false);
            }
        #endregion
        #region M:OnIsFloatingWindowDragWithinChanged(DependencyObject,DependencyPropertyChangedEventArgs)
        private static void OnIsFloatingWindowDragWithinChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = sender as UIElement;
            if (source != null) {
                source.RaiseEvent(new RoutedEventArgs(IsFloatingWindowDragWithinChangedEvent));
                }
            }
        #endregion
        #region M:SetIsFloatingWindowDragWithin(IEnumerable<DependencyObject>,Boolean)
        internal static void SetIsFloatingWindowDragWithin(IEnumerable<DependencyObject> values, Boolean value) {
            var box = Boxes.Box(value);
            foreach (var source in values) {
                source.SetValue(IsFloatingWindowDragWithinProperty, box);
                }
            }
        #endregion
        #region M:OnLeftOrTopChanged(DependencyObject,DependencyPropertyChangedEventArgs)
        private static void OnLeftOrTopChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((UIElement)sender).RaiseEvent(new RoutedEventArgs(LocationChangedEvent));
            }
        #endregion

        private static void OnIsUndockingTabChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var newValue = (Boolean)e.NewValue;
            var floatingWindow = sender as FloatingWindow;
            if (floatingWindow == null)
                return;
            floatingWindow.UpdateClipRegion(newValue ? ClipRegionChangeType.FromUndockSingleTab : ClipRegionChangeType.FromPropertyChange);
            if (newValue || floatingWindow.ViewSite == null)
                return;
            var viewSite = floatingWindow.ViewSite;
            var num = 0;
            var view = viewSite.Find<View>(v => v.IsVisible, num != 0);
            if (view == null || view.Parent == null)
                return;
            view.Parent.TryCollapse();
            }

        public static void ShowWindowMenu(FloatingWindow window, Point screenPoint) {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            var source = PresentationSource.FromVisual(window) as HwndSource;
            var dataContext = window.DataContext as FloatSite;
            var canMinimize = false;
            if (dataContext != null)
                canMinimize = dataContext.IsIndependent;
            if (source == null)
                return;
            CustomChromeWindow.ShowWindowMenu(source, screenPoint, canMinimize);
            }

        public static void ShowWindowMenu(HwndSource source, Point screenPoint) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var rootVisual = source.RootVisual as FloatingWindow;
            if (rootVisual == null)
                return;
            var canMinimize = false;
            var dataContext = rootVisual.DataContext as FloatSite;
            if (dataContext != null)
                canMinimize = dataContext.IsIndependent;
            CustomChromeWindow.ShowWindowMenu(source, screenPoint, canMinimize);
            }

        /// <summary>Creates and returns a <see cref="T:System.Windows.Automation.Peers.WindowAutomationPeer" /> object for this <see cref="T:System.Windows.Window" />.</summary>
        /// <returns>A <see cref="T:System.Windows.Automation.Peers.WindowAutomationPeer" /> object for this <see cref="T:System.Windows.Window" />.</returns>
        protected override AutomationPeer OnCreateAutomationPeer() {
            return new FloatingWindowAutomationPeer(this);
            }

        /// <summary>Raises the <see cref="E:System.Windows.Window.SourceInitialized" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e) {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            ((IKeyboardInputSink)hwndSource).RegisterKeyboardInputSink(new MnemonicForwardingKeyboardInputSink(this));
            UtilityMethods.ModifyStyle(hwndSource.Handle, 0, Int32.MinValue);
            UpdateClipRegion(ClipRegionChangeType.FromPropertyChange);
            base.OnSourceInitialized(e);
            }

        /// <summary>Raises the <see cref="E:System.Windows.FrameworkElement.Initialized" /> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized" /> is set to true internally. </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);
            var handle = new WindowInteropHelper(this).Handle;
            if (DockManager.Instance.UndockingInformation == null || !NativeMethods.IsLeftButtonPressed())
                return;
            var undockingSingleTab = MultiSelectionManager.Instance.SelectedElementCount <= 1 && DockManager.Instance.UndockingInformation.UndockMode == UndockMode.Tab;
            if (undockingSingleTab) {
                SetIsUndockingTab(this, true);
                LayoutUpdated += OnLayoutUpdated;
                }
            var dragPoint = DockManager.Instance.UndockingInformation.Location;
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => {
                var messagePoint = DragUndockHeader.GetMessagePoint();
                if (undockingSingleTab)
                    UpdateSingleTabWindowPosition(handle, dragPoint, messagePoint);
                else
                    UpdateWindowPosition(handle, dragPoint, messagePoint);
                NativeMethods.SendMessage(handle, 161, new IntPtr(2), NativeMethods.MakeParam((Int32)messagePoint.X, (Int32)messagePoint.Y));
                //return (object) null;
            }));
            }

        private static void UpdateWindowPosition(IntPtr handle, Point originalPoint, Point currentPoint) {
            RECT lpRect;
            NativeMethods.GetWindowRect(handle, out lpRect);
            var num1 = currentPoint.X - originalPoint.X;
            var num2 = currentPoint.Y - originalPoint.Y;
            NativeMethods.SetWindowPos(handle, IntPtr.Zero, lpRect.Left + (Int32)num1, lpRect.Top + (Int32)num2, 0, 0, 789);
            }

        private void UpdateSingleTabWindowPosition(IntPtr handle, Point originalPoint, Point currentPoint) {
            var descendant = this.FindDescendant<TabItem>();
            if (descendant == null) {
                UpdateWindowPosition(handle, originalPoint, currentPoint);
                }
            else {
                var rect = RectangleWithinWindow(descendant);
                NativeMethods.SetWindowPos(handle, IntPtr.Zero, (Int32)currentPoint.X - (rect.Left + rect.Width / 2), (Int32)currentPoint.Y - (rect.Top + rect.Height / 2), 0, 0, 789);
                }
            }

        protected override IntPtr HwndSourceHook(IntPtr h, Int32 m, IntPtr w, IntPtr l, ref Boolean r) {
            if (m <= WM_NCLBUTTONDBLCLK) {
                if (m != WM_SHOWWINDOW) {
                    if (m == WM_NCLBUTTONDBLCLK) {
                        WmNcLButtonDblClk(h, w, ref r);
                        return IntPtr.Zero;
                        }
                    }
                else
                    {
                    WmShowWindow(h, l);
                    return IntPtr.Zero;
                    }
                }
            else if (m != WM_EXITSIZEMOVE) {
                if (m == WM_APPCOMMAND) {
                    var e = new WindowsMessageEventArgs(AppCommandMessageReceived, new MSG {
                        hwnd = h,
                        lParam = l,
                        wParam = w,
                        message = m
                        });
                    RaiseEvent(e);
                    r = e.Handled;
                    return IntPtr.Zero;
                    }
                }
            else
                {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => SetIsUndockingTab(this, false)), DispatcherPriority.Normal);
                return IntPtr.Zero;
                }
            return base.HwndSourceHook(h, m, w, l, ref r);
            }

        private void WmNcLButtonDblClk(IntPtr hWnd, IntPtr wParam, ref Boolean handled) {
            if (!NativeMethods.IsKeyPressed(17) || wParam.ToInt32() != 2)
                return;
            var dataContext = DataContext as ViewElement;
            if (dataContext == null)
                return;
            ViewCommands.ToggleDocked.Execute(dataContext, this);
            handled = true;
            }

        private void WmShowWindow(IntPtr hWnd, IntPtr lParam) {
            if (lParam.ToInt32() == 3) {
                if (WindowState == WindowState.Maximized) {
                    ShowActivated = true;
                    NativeMethods.ShowWindow(hWnd, 3);
                    }
                else
                    NativeMethods.ShowWindow(hWnd, 9);
                }
            else {
                if (lParam.ToInt32() != 1)
                    return;
                NativeMethods.ShowWindow(hWnd, 6);
                }
            }

        protected override void OnWindowPosChanged(IntPtr hWnd, Int32 showCmd, Int32Rect rcNormalPosition) {
            if (IsUpdatingWindowPlacement)
                return;
            IsUpdatingWindowPlacement = true;
            try {
                var viewSite = ViewSite;
                if (viewSite == null)
                    return;
                Point devicePoint;
                Size deviceSize;
                if (showCmd == 3 || showCmd == 2) {
                    devicePoint = Screen.WorkAreaToScreen(new Point(rcNormalPosition.X, rcNormalPosition.Y));
                    deviceSize = new Size(rcNormalPosition.Width, rcNormalPosition.Height);
                    }
                else {
                    RECT lpRect;
                    NativeMethods.GetWindowRect(hWnd, out lpRect);
                    devicePoint = lpRect.Position;
                    deviceSize = lpRect.Size;
                    }
                var logicalUnits1 = devicePoint.DeviceToLogicalUnits();
                var logicalUnits2 = deviceSize.DeviceToLogicalUnits();
                Int32 display;
                Rect relativePosition;
                Screen.AbsoluteRectToRelativeRect(logicalUnits1.X, logicalUnits1.Y, logicalUnits2.Width, logicalUnits2.Height, out display, out relativePosition);
                viewSite.Display = display;
                viewSite.FloatingLeft = relativePosition.Left;
                viewSite.FloatingTop = relativePosition.Top;
                viewSite.FloatingWidth = logicalUnits2.Width;
                viewSite.FloatingHeight = logicalUnits2.Height;
                }
            finally {
                IsUpdatingWindowPlacement = false;
                }
            }

        protected override Boolean UpdateClipRegionCore(IntPtr hWnd, Int32 showCmd, ClipRegionChangeType changeType, Int32Rect currentBounds) {
            if (base.UpdateClipRegionCore(hWnd, showCmd, changeType, currentBounds))
                return true;
            if (changeType != ClipRegionChangeType.FromUndockSingleTab)
                return false;
            SetTabRegion(hWnd, currentBounds);
            return true;
            }

        private void SetTabRegion(IntPtr hWnd, Int32Rect currentBounds) {
            var element1 = new List<GroupControlTabItem>(this.FindDescendants<GroupControlTabItem>()).FirstOrDefault(item => item.ActualWidth != 0.0);
            GroupControl groupControl = null;
            FrameworkElement element2 = null;
            if (element1 != null) {
                groupControl = element1.FindAncestor<GroupControl>();
                if (groupControl != null)
                    element2 = groupControl.GetContentPanel();
                }
            if (element1 == null || groupControl == null || element2 == null) {
                SetRoundRect(hWnd, currentBounds.Width, currentBounds.Height);
                }
            else {
                var zero = IntPtr.Zero;
                var num = IntPtr.Zero;
                try {
                    var rect1 = RectangleWithinWindow(element1);
                    var rect2 = RectangleWithinWindow(element2);
                    ++rect2.Height;
                    var radiusRectRegion = ComputeCornerRadiusRectRegion(rect1.ToInt32Rect(), GroupControlTabItem.GetCornerRadius(element1));
                    num = ComputeCornerRadiusRectRegion(rect2.ToInt32Rect(), groupControl.ContentCornerRadius);
                    NativeMethods.CombineRgn(radiusRectRegion, radiusRectRegion, num, NativeMethods.CombineMode.RGN_OR);
                    NativeMethods.SetWindowRgn(hWnd, radiusRectRegion, NativeMethods.IsWindowVisible(hWnd));
                    }
                finally {
                    if (num != IntPtr.Zero)
                        NativeMethods.DeleteObject(num);
                    }
                }
            }

        private RECT RectangleWithinWindow(FrameworkElement element) {
            var rect = new RECT();
            var deviceUnits = TransformToDescendant(element).Transform(new Point(0.0, 0.0)).LogicalToDeviceUnits();
            rect.Left = (Int32)(-deviceUnits.X);
            rect.Top = (Int32)(-deviceUnits.Y);
            rect.Width = (Int32)(element.ActualWidth * DpiHelper.LogicalToDeviceUnitsScalingFactorX);
            rect.Height = (Int32)(element.ActualHeight * DpiHelper.LogicalToDeviceUnitsScalingFactorY);
            return rect;
            }

        private class MnemonicForwardingKeyboardInputSink : UIElement, IKeyboardInputSink {
            private Window Window { get; }

            IKeyboardInputSite IKeyboardInputSink.KeyboardInputSite { get; set; }

            public MnemonicForwardingKeyboardInputSink(Window window) {
                Window = window;
                }

            #region M:IKeyboardInputSink.HasFocusWithin:Boolean
            Boolean IKeyboardInputSink.HasFocusWithin() {
                return false;
                }
            #endregion
            #region M:IKeyboardInputSink.RegisterKeyboardInputSink(IKeyboardInputSink):IKeyboardInputSite
            IKeyboardInputSite IKeyboardInputSink.RegisterKeyboardInputSink(IKeyboardInputSink sink) {
                throw new NotSupportedException();
                }
            #endregion
            #region M:IKeyboardInputSink.TabInto(TraversalRequest):Boolean
            Boolean IKeyboardInputSink.TabInto(TraversalRequest request) {
                return false;
                }
            #endregion
            #region M:IKeyboardInputSink.TranslateAccelerator(MSG,ModifierKeys):Boolean
            Boolean IKeyboardInputSink.TranslateAccelerator(ref MSG msg, ModifierKeys modifiers) {
                return false;
                }
            #endregion
            #region M:IKeyboardInputSink.TranslateChar(MSG,ModifierKeys):Boolean
            Boolean IKeyboardInputSink.TranslateChar(ref MSG msg, ModifierKeys modifiers) {
                return false;
                }
            #endregion

            Boolean IKeyboardInputSink.OnMnemonic(ref MSG msg, ModifierKeys modifiers) {
                switch (msg.message) {
                    case 262:
                    case 263:
                    var key = new String((Char)(Int32)msg.wParam, 1);
                    if ((key != null) && (key.Length > 0)) {
                        var hwnd = new WindowInteropHelper(Window).Owner;
                        if (hwnd == IntPtr.Zero)
                            hwnd = ViewManager.Instance.FloatingWindowManager.OwnerWindow;
                        if (hwnd != IntPtr.Zero) {
                            var hwndSource = HwndSource.FromHwnd(hwnd);
                            if (hwndSource != null && AccessKeyManager.IsKeyRegistered(hwndSource, key)) {
                                AccessKeyManager.ProcessKey(hwndSource, key, false);
                                return true;
                                }
                            break;
                            }
                        break;
                        }
                    break;
                    }
                return false;
                }
            }
        }
    }
