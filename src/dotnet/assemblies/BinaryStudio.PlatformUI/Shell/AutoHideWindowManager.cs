using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class AutoHideWindowManager
        {
        private View autoHideWindowElement;

        private AutoHideWindow AutoHideWindow { get; set; }

        private AutoHideChannelControl AutoHideChannelControl { get; set; }

        private AutoHideTabItem AutoHideTabItem { get; set; }

        private MouseVirtualCaptureObserver MouseObserver { get; set; }

        public View AutoHideWindowElement
            {
            get
                {
                return autoHideWindowElement;
                }
            private set
                {
                if (autoHideWindowElement != null)
                    {
                    autoHideWindowElement.ParentChanged -= OnElementParentOrVisibilityChanged;
                    autoHideWindowElement.IsVisibleChanged -= OnElementParentOrVisibilityChanged;
                    autoHideWindowElement.IsSelectedChanged -= OnElementParentOrVisibilityChanged;
                    autoHideWindowElement.IsSelected = false;
                    }
                autoHideWindowElement = value;
                if (autoHideWindowElement == null)
                    return;
                autoHideWindowElement.IsSelected = true;
                autoHideWindowElement.IsVisibleChanged += OnElementParentOrVisibilityChanged;
                autoHideWindowElement.ParentChanged += OnElementParentOrVisibilityChanged;
                autoHideWindowElement.IsSelectedChanged += OnElementParentOrVisibilityChanged;
                }
            }

        public Boolean IsAutoHideWindowShown
            {
            get
                {
                if (AutoHideWindow != null)
                    return AutoHideWindow.Visibility == Visibility.Visible;
                return false;
                }
            }

        public AutoHideWindowManager()
            {
            ViewManager.Instance.ActiveViewChanged += OnActiveViewChanged;
            }

        private void OnActiveViewChanged(Object sender, EventArgs e)
            {
            if (!IsAutoHideWindowShown)
                return;
            var activeView = ViewManager.Instance.ActiveView;
            if (activeView != AutoHideWindowElement)
                {
                if (activeView == null || !AutoHideChannel.IsAutoHidden(activeView))
                    {
                    if (MouseVirtualCaptureObserver.IsMouseOverAny((Visual)AutoHideWindow, (Visual)AutoHideTabItem))
                        {
                        if (MouseObserver != null)
                            return;
                        StartMouseObserver(AutoHideTabItem);
                        }
                    else
                        CloseAutoHideWindow();
                    }
                else
                    CloseAutoHideWindow();
                }
            else
                StopMouseObserver();
            }

        public void ShowAutoHideWindow(AutoHideTabItem item, View view)
            {
            if (view == AutoHideWindowElement)
                return;
            CloseAutoHideWindow();
            var ancestor1 = item.FindAncestor<AutoHideChannelControl>();
            if (ancestor1 == null)
                return;
            var ancestor2 = item.FindAncestor<AutoHideRootControl>();
            if (ancestor2 == null)
                return;
            AutoHideWindowElement = view;
            AutoHideTabItem = item;
            AutoHideChannelControl = ancestor1;
            if (AutoHideWindow == null)
                AutoHideWindow = new AutoHideWindow();
            else
                AutoHideWindow.Visibility = Visibility.Visible;
            AutoHideWindow.DataContext = view;
            AutoHideWindow.DockRootElement = ancestor2.DockRoot;
            AutoHideChannelControl.AutoHideSlideout = AutoHideWindow;
            LayoutSynchronizer.Update(AutoHideWindow);
            if (!AutoHideWindowElement.IsActive)
                StartMouseObserver(item);
            AutoHideWindow.RaiseEvent(new ViewEventArgs(AutoHideWindow.SlideOpenedEvent, AutoHideWindowElement));
            }

        private void StartMouseObserver(AutoHideTabItem item)
            {
            MouseObserver = new MouseVirtualCaptureObserver(item, AutoHideWindow);
            MouseObserver.LostVirtualMouseCapture += OnLostVirtualMouseCapture;
            }

        private void StopMouseObserver()
            {
            if (MouseObserver == null)
                return;
            MouseObserver.Dispose();
            MouseObserver = null;
            }

        private void OnLostVirtualMouseCapture(Object sender, EventArgs args)
            {
            CloseAutoHideWindow();
            }

        private void OnElementParentOrVisibilityChanged(Object sender, EventArgs args)
            {
            CloseAutoHideWindow();
            }

        public void CloseAutoHideWindow()
            {
            if (!IsAutoHideWindowShown)
                return;
            var hideWindowElement = AutoHideWindowElement;
            StopMouseObserver();
            if (Keyboard.FocusedElement != null && FocusOperations.IsKeyboardFocusWithin(AutoHideWindow))
                Keyboard.ClearFocus();
            AutoHideChannelControl.AutoHideSlideout = null;
            AutoHideWindow.Visibility = Visibility.Collapsed;
            AutoHideWindow.DataContext = null;
            AutoHideWindowElement = null;
            AutoHideTabItem = null;
            AutoHideChannelControl = null;
            AutoHideWindow.RaiseEvent(new ViewEventArgs(AutoHideWindow.SlideClosedEvent, hideWindowElement));
            }

        private class MouseVirtualCaptureObserver : DisposableObject
            {
            private const Int32 VirtualCaptureTimerIntervalMilliseconds = 50;

            private AutoHideWindow Window { get; }

            private AutoHideTabItem TabItem { get; }

            private DispatcherTimer IsMouseOverTimer { get; set; }

            private DispatcherTimer MouseExitedTimer { get; set; }

            public event EventHandler LostVirtualMouseCapture;

            public MouseVirtualCaptureObserver(AutoHideTabItem tabItem, AutoHideWindow window)
                {
                Window = window;
                TabItem = tabItem;
                BroadcastMessageMonitor.Instance.Activated += OnApplicationActivated;
                BroadcastMessageMonitor.Instance.Deactivated += OnApplicationDeactivated;
                StartIsMouseOverTimer();
                }

            private void StartIsMouseOverTimer()
                {
                if (IsMouseOverTimer != null)
                    return;
                IsMouseOverTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(VirtualCaptureTimerIntervalMilliseconds), DispatcherPriority.ApplicationIdle, OnUpdateIsMouseOver, Window.Dispatcher);
                }

            private void StopIsMouseOverTimer()
                {
                if (IsMouseOverTimer == null)
                    return;
                IsMouseOverTimer.Stop();
                IsMouseOverTimer = null;
                }

            internal static Boolean IsMouseOverAny(params Visual[] visuals)
                {
                var cursorPos = NativeMethods.GetCursorPos();
                foreach (var visual in visuals)
                    {
                    if (visual.IsConnectedToPresentationSource())
                        {
                        var point = visual.PointFromScreen(cursorPos);
                        var hitVisual = false;
                        UtilityMethods.HitTestVisibleElements(visual, result =>
                        {
                            hitVisual = true;
                            return HitTestResultBehavior.Stop;
                        }, new PointHitTestParameters(point));
                        if (hitVisual)
                            return true;
                        var uiElement = visual as UIElement;
                        if (uiElement != null && uiElement.IsMouseCaptureWithin)
                            return true;
                        }
                    }
                return false;
                }

            private void OnUpdateIsMouseOver(Object sender, EventArgs args)
                {
                if (!IsMouseOverAny((Visual)Window, (Visual)TabItem))
                    StartMouseExitedTimer();
                else
                    StopMouseExitedTimer();
                }

            private void StartMouseExitedTimer()
                {
                if (MouseExitedTimer != null)
                    return;
                MouseExitedTimer = new DispatcherTimer(ViewManager.Instance.Preferences.AutoHideMouseExitGracePeriod, DispatcherPriority.Input, OnMouseExited, Window.Dispatcher);
                }

            private void StopMouseExitedTimer()
                {
                if (MouseExitedTimer == null)
                    return;
                MouseExitedTimer.Stop();
                MouseExitedTimer = null;
                }

            private void OnMouseExited(Object sender, EventArgs args)
                {
                StopMouseExitedTimer();
                // ISSUE: reference to a compiler-generated field
                LostVirtualMouseCapture.RaiseEvent(this);
                }

            private void OnApplicationDeactivated(Object sender, EventArgs args)
                {
                StopMouseExitedTimer();
                StopIsMouseOverTimer();
                }

            private void OnApplicationActivated(Object sender, EventArgs args)
                {
                StartIsMouseOverTimer();
                }

            protected override void DisposeManagedResources()
                {
                StopMouseExitedTimer();
                StopIsMouseOverTimer();
                BroadcastMessageMonitor.Instance.Activated -= OnApplicationActivated;
                BroadcastMessageMonitor.Instance.Deactivated -= OnApplicationDeactivated;
                }
            }
        }
    }