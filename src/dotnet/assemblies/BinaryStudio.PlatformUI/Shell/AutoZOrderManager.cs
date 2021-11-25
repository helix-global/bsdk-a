using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Shell
    {
    public static class AutoZOrderManager
        {
        private static DispatcherTimer autoZOrderTimer;
        private static Window currentDragOverWindow;

        public static Window CurrentDragOverWindow
            {
            get
                {
                return currentDragOverWindow;
                }
            set
                {
                if (currentDragOverWindow == value)
                    return;
                StopTimer();
                currentDragOverWindow = value;
                if (currentDragOverWindow == null)
                    return;
                autoZOrderTimer = new DispatcherTimer(ViewManager.Instance.Preferences.AutoZOrderDelay, DispatcherPriority.Normal, OnAutoZOrderTimer, currentDragOverWindow.Dispatcher);
                autoZOrderTimer.Start();
                }
            }

        public static void AdornersCleared(Window window)
            {
            if (CurrentDragOverWindow != window)
                return;
            CurrentDragOverWindow = null;
            }

        private static void OnAutoZOrderTimer(Object obj, EventArgs args)
            {
            StopTimer();
            if (CurrentDragOverWindow == null)
                return;
            var num = 1;
            var draggedWindowHandle = DockManager.Instance.DraggedWindowHandle;
            var handle = new WindowInteropHelper(CurrentDragOverWindow).Handle;
            var floatingWindowManager = ViewManager.Instance.FloatingWindowManager;
            foreach (Window floatingWindow in floatingWindowManager.FloatingWindows)
                {
                var windowInteropHelper = new WindowInteropHelper(floatingWindow);
                if (windowInteropHelper.Handle != draggedWindowHandle && windowInteropHelper.Owner == handle)
                    ++num;
                }
            if (floatingWindowManager.GetWindowZOrder(CurrentDragOverWindow) == num)
                return;
            NativeMethods.SetWindowPos(handle, draggedWindowHandle, 0, 0, 0, 0, 16403);
            if (ViewManager.Instance.CurrentDragUndockHeader == null)
                return;
            DockManager.Instance.ClearAdorners();
            ViewManager.Instance.CurrentDragUndockHeader.RaiseDragAbsolute(NativeMethods.GetCursorPos());
            }

        private static void StopTimer()
            {
            if (autoZOrderTimer == null)
                return;
            autoZOrderTimer.Stop();
            autoZOrderTimer = null;
            }
        }
    }