using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell
    {
    public static class UtilityMethods
        {
        private static Point lastContentPos = new Point(Double.NaN, Double.NaN);
        private static Point currentFloatPos = new Point(Double.NaN, Double.NaN);
        private const Double FloatStepX = 20.0;
        private const Double FloatStepY = 20.0;
        private const Int32 PositionRetries = 100;

        internal static void AddPresentationSourceCleanupAction(UIElement element, Action handler)
            {
            var relayHandler = (SourceChangedEventHandler)null;
            relayHandler = (sender, args) =>
            {
                if (args.NewSource != null)
                    return;
                if (!element.Dispatcher.HasShutdownStarted)
                    handler();
                PresentationSource.RemoveSourceChangedHandler(element, relayHandler);
            };
            PresentationSource.AddSourceChangedHandler(element, relayHandler);
            }

        public static void HitTestVisibleElements(Visual visual, HitTestResultCallback resultCallback, HitTestParameters parameters)
            {
            VisualTreeHelper.HitTest(visual, ExcludeNonVisualElements, resultCallback, parameters);
            }

        private static HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
            {
            if (!(potentialHitTestTarget is Visual))
                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            var uiElement = potentialHitTestTarget as UIElement;
            return uiElement == null || uiElement.IsVisible && uiElement.IsEnabled ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            }

        internal static Point CalculateFloatingPosition(ViewElement element, WindowProfile profile)
            {
            var point = new Point(element.FloatingLeft, element.FloatingTop);
            var viewGroup = element as ViewGroup;
            if (viewGroup is NestedGroup && viewGroup.SelectedElement != null)
                {
                point.X = viewGroup.SelectedElement.FloatingLeft;
                point.Y = viewGroup.SelectedElement.FloatingTop;
                }
            if ((point.X.IsNearlyEqual(Double.NaN) || point.Y.IsNearlyEqual(Double.NaN)) && (Application.Current != null && Application.Current.MainWindow != null))
                {
                Rect screenSubRect;
                Rect monitorRect;
                Screen.FindMaximumSingleMonitorRectangle(Application.Current.MainWindow.GetDeviceRect(), out screenSubRect, out monitorRect);
                var logicalUnits1 = monitorRect.DeviceToLogicalUnits();
                if (ViewManager.Instance.MainWindowContent != null && ViewManager.Instance.MainWindowContent.IsConnectedToPresentationSource() && !logicalUnits1.Contains(lastContentPos))
                    {
                    var logicalUnits2 = ViewManager.Instance.MainWindowContent.PointToScreen(new Point(0.0, 0.0)).DeviceToLogicalUnits();
                    currentFloatPos = logicalUnits2;
                    lastContentPos = logicalUnits2;
                    }
                else
                    {
                    if (currentFloatPos.X.IsNonreal())
                        currentFloatPos.X = logicalUnits1.Left;
                    if (currentFloatPos.Y.IsNonreal())
                        currentFloatPos.Y = logicalUnits1.Top;
                    }
                point = CalculateFloatingPosition(element, profile, logicalUnits1, currentFloatPos.X, currentFloatPos.Y);
                currentFloatPos.X = point.X + 20.0;
                currentFloatPos.Y = point.Y + 20.0;
                }
            return point;
            }

        internal static Point CalculateFloatingPosition(FloatSite oldSite, ViewElement element, WindowProfile profile)
            {
            var floatingLeft = oldSite.FloatingLeft;
            var floatingTop = oldSite.FloatingTop;
            var point = new Point();
            if (element.FloatingLeft.IsNearlyEqual(Double.NaN) || element.FloatingTop.IsNearlyEqual(Double.NaN) || element.FloatingLeft.IsNearlyEqual(floatingLeft) && element.FloatingTop.IsNearlyEqual(floatingTop))
                {
                var floatingWindow = ViewManager.Instance.FloatingWindowManager.TryGetFloatingWindow(oldSite);
                if (floatingWindow != null)
                    {
                    Rect screenSubRect;
                    Rect monitorRect;
                    Screen.FindMaximumSingleMonitorRectangle(floatingWindow.GetDeviceRect(), out screenSubRect, out monitorRect);
                    var logicalUnits = monitorRect.DeviceToLogicalUnits();
                    return CalculateFloatingPosition(element, profile, logicalUnits, floatingLeft, floatingTop);
                    }
                }
            point.X = element.FloatingLeft;
            point.Y = element.FloatingTop;
            return point;
            }

        private static Point CalculateFloatingPosition(ViewElement element, WindowProfile profile, Rect monitorRect, Double startingLeft, Double startingTop)
            {
            var point = new Point(startingLeft, startingTop);
            for (var index = 0; index < 100 && IsFloatSiteAtPosition(profile, point.X, point.Y); ++index)
                {
                point.X += 20.0;
                point.Y += 20.0;
                }
            if (element.FloatingWidth / 3.0 + point.X > monitorRect.Right)
                point.X = 0.0;
            if (element.FloatingHeight / 3.0 + point.Y > monitorRect.Bottom)
                point.Y = 0.0;
            if (point.X < monitorRect.Left)
                point.X = monitorRect.Left;
            if (point.Y < monitorRect.Top)
                point.Y = monitorRect.Top;
            return point;
            }

        private static Boolean IsFloatSiteAtPosition(WindowProfile profile, Double floatLeft, Double floatTop)
            {
            foreach (var floatSite in new List<FloatSite>(profile.FindAll<FloatSite>()))
                {
                if (floatSite.IsVisible && floatSite.FloatingLeft.IsNearlyEqual(floatLeft) && floatSite.FloatingTop.IsNearlyEqual(floatTop))
                    return true;
                }
            return false;
            }

        internal static Boolean ModifyStyle(IntPtr hWnd, Int32 styleToRemove, Int32 styleToAdd)
            {
            var windowLong = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL.STYLE);
            var dwNewLong = windowLong & ~styleToRemove | styleToAdd;
            if (dwNewLong == windowLong)
                return false;
            NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL.STYLE, dwNewLong);
            return true;
            }

        internal static HwndSource FindTopLevelHwndSource(UIElement element)
            {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(element);
            if (hwndSource != null && IsChildWindow(hwndSource.Handle))
                hwndSource = HwndSource.FromHwnd(FindTopLevelWindow(hwndSource.Handle));
            return hwndSource;
            }

        internal static Boolean IsChildWindow(IntPtr hWnd)
            {
            return (NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL.STYLE) & 1073741824) == 1073741824;
            }

        internal static IntPtr FindTopLevelWindow(IntPtr hWnd)
            {
            while (hWnd != IntPtr.Zero && IsChildWindow(hWnd))
                hWnd = NativeMethods.GetParent(hWnd);
            return hWnd;
            }

        internal static void RestoreWindow(View view)
            {
            var window = WindowFromView(view);
            if (window == null || window.WindowState != WindowState.Minimized)
                return;
            window.WindowState = WindowState.Normal;
            }

        public static Window WindowFromView(View view)
            {
            var rootElement = ViewElement.FindRootElement(view);
            var site = rootElement as FloatSite;
            if (site != null)
                return ViewManager.Instance.FloatingWindowManager.TryGetFloatingWindow(site);
            if (rootElement is MainSite && Application.Current != null)
                return Application.Current.MainWindow;
            return null;
            }
        }
    }