using System;
using System.Threading;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Shell
    {
    public static class KeyboardStateManager
        {
        private static Boolean isControlPressed;
        private static Timer keyboardStateTimer;
        private static DragUndockHeader currentDragUndockHeader;

        public static DragUndockHeader CurrentDragUndockHeader
            {
            get
                {
                return currentDragUndockHeader;
                }
            set
                {
                currentDragUndockHeader = value;
                if (currentDragUndockHeader != null)
                    {
                    IsMonitoringKeyboard = true;
                    }
                else
                    {
                    IsMonitoringKeyboard = false;
                    isControlPressed = false;
                    }
                }
            }

        private static Boolean IsMonitoringKeyboard
            {
            get
                {
                return keyboardStateTimer != null;
                }
            set
                {
                if (value)
                    {
                    if (keyboardStateTimer != null)
                        keyboardStateTimer.Dispose();
                    keyboardStateTimer = new Timer(OnKeyboardStateTimer, null, 0, 200);
                    }
                else
                    {
                    if (keyboardStateTimer == null)
                        return;
                    keyboardStateTimer.Dispose();
                    keyboardStateTimer = null;
                    }
                }
            }

        private static void OnKeyboardStateTimer(Object state)
            {
            if (NativeMethods.IsKeyPressed(17))
                {
                if (!isControlPressed && CurrentDragUndockHeader != null)
                    CurrentDragUndockHeader.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                   {
                       DockManager.Instance.ClearAdorners();
              //return (object) null;
          }), null);
                isControlPressed = true;
                }
            else
                {
                if (isControlPressed && CurrentDragUndockHeader != null)
                    CurrentDragUndockHeader.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                   {
                       if (CurrentDragUndockHeader != null)
                           CurrentDragUndockHeader.RaiseDragAbsolute(NativeMethods.GetCursorPos());
              //return (object) null;
          }), null);
                isControlPressed = false;
                }
            }
        }
    }