using System;
using System.Windows.Interop;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal sealed class BroadcastMessageMonitor
        {
        private HwndSource hwndSource;
        private Boolean isActive;
        private static BroadcastMessageMonitor instance;

        public static BroadcastMessageMonitor Instance
            {
            get
                {
                return instance ?? (instance = new BroadcastMessageMonitor());
                }
            }

        internal HwndSource HwndSource
            {
            get
                {
                return hwndSource;
                }
            set
                {
                if (hwndSource == value)
                    return;
                if (hwndSource != null)
                    hwndSource.RemoveHook(WndProcHook);
                hwndSource = value;
                if (hwndSource == null)
                    return;
                hwndSource.AddHook(WndProcHook);
                IsActive = IsApplicationActive();
                }
            }

        public Boolean IsActive
            {
            get
                {
                if (HwndSource == null)
                    return IsApplicationActive();
                return isActive;
                }
            private set
                {
                if (isActive == value)
                    return;
                isActive = value;
                if (isActive)
                    OnActivated();
                else
                    OnDeactivated();
                }
            }

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public event EventHandler DisplayChange;

        private static Boolean IsApplicationActive()
            {
            var foregroundWindow = NativeMethods.GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
                return false;
            UInt32 processId;
            NativeMethods.GetWindowThreadProcessId(foregroundWindow, out processId);
            var currentProcessId = NativeMethods.GetCurrentProcessId();
            return (Int32)processId == (Int32)currentProcessId;
            }

        private IntPtr WndProcHook(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
            {
            if (msg != 28)
                {
                if (msg == 126)
                    OnDisplayChange();
                }
            else
                IsActive = wParam != IntPtr.Zero;
            return IntPtr.Zero;
            }

        private void OnActivated()
            {
            // ISSUE: reference to a compiler-generated field
            Activated.RaiseEvent(this);
            }

        private void OnDeactivated()
            {
            // ISSUE: reference to a compiler-generated field
            Deactivated.RaiseEvent(this);
            }

        private void OnDisplayChange()
            {
            // ISSUE: reference to a compiler-generated field
            DisplayChange.RaiseEvent(this);
            }
        }
    }